using Dapper;
using Entities.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using Npgsql;
using Order.Application.Models.ViewModels;
using Order.Common.Resources;
using Order.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Wallet.Infrastructure.Services
{
    public class OrderService : ServiceBase<OrderService>, IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _Context;

        public OrderService(IConfiguration configuration, ApplicationDbContext dbContext, ILogger<OrderService> logger) : base(logger)
        {
            _configuration = configuration;
            _Context = dbContext;
        }

        public async Task<ServiceResult> AdjustDiscount(int userId, string code, decimal amount)
        {
            try
            {
                using (IDbConnection dbConnection = _Context.Connection)
                {
                    dbConnection.Open();

                    // Query to get discount information
                    string query = "SELECT id, quantity, expire_date, percent FROM discounts WHERE code = @Code";

                    // Execute the query
                    var discountInfo = await dbConnection.QueryFirstOrDefaultAsync(query, new { Code = code });

                    if (discountInfo is null)
                        return BadRequest(ErrorCodeEnum.CodeNotFound, Resource.CodeNotFound, null);

                    var validate = await ValidateDiscount(code, null, userId);

                    if (validate != null)
                        return validate;

                    // Calculate new price
                    var newPrice = amount * (100 - discountInfo.percent) / 100;

                    return Ok(new { DiscountId = discountInfo.id, NewPrice = newPrice, Percent = discountInfo.percent });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }

        private async Task<ServiceResult> CheckBookExists(int bookId)
        {
            try
            {
                using (IDbConnection dbConnection = _Context.Connection)
                {
                    dbConnection.Open();

                    // Query to check if the book exists
                    var bookCheckQuery = "SELECT 1 FROM books WHERE id = @BookId AND is_delete = false";

                    // Execute the query
                    var bookCheck = await dbConnection.ExecuteScalarAsync<int?>(bookCheckQuery, new { BookId = bookId });

                    if (bookCheck is null)
                        return Ok(false);

                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }

        public async Task<ServiceResult> UserPurchased(int userId, int bookId)
        {
            try
            {
                using (IDbConnection dbConnection = _Context.Connection)
                {
                    dbConnection.Open();

                    // Query to check if the user has purchased the book
                    string query = "SELECT 1 FROM user_books WHERE user_id = @UserId AND book_id = @BookId";

                    // Execute the query
                    var result = await dbConnection.ExecuteScalarAsync<int?>(query, new { UserId = userId, BookId = bookId });

                    // Check the result and return accordingly
                    if (result != null)
                    {
                        return Ok(true); // The user has purchased the book
                    }
                    else
                    {
                        return Ok(false); // The user has not purchased the book
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }
        public async Task<ServiceResult> PurchaseBook(int userId, PurchaseBookViewModel model)
        {
            using (IDbConnection dbConnection = _Context.Connection)
            {
                dbConnection.Open();
                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        var userPurchased = await UserPurchased(userId, model.BookId);

                        if (userPurchased.Data is true)
                            return BadRequest(ErrorCodeEnum.BookPurchased, Resource.BookPurchased, null);

                        var checkBook = await CheckBookExists(model.BookId);

                        if (checkBook.Data is false)
                            return BadRequest(ErrorCodeEnum.BookNotFound, Resource.BookNotFound, null);

                        // Query to get book amount
                        string amountQuery = "SELECT price FROM books WHERE id = @BookId AND is_delete = false";

                        // Execute the query
                        var amount = await dbConnection.ExecuteScalarAsync<decimal>(amountQuery, new { BookId = model.BookId });

                        if (model.DiscountId != null)
                        {
                            var validate = await ValidateDiscount(null, model.DiscountId, userId);

                            if (validate != null)
                                return validate;

                            // Query to get discount percentage
                            string discountquery = "SELECT percent FROM discounts WHERE id = @DiscountId";

                            // Execute the query
                            var percent = await dbConnection.ExecuteScalarAsync<decimal>(discountquery, new { DiscountId = model.DiscountId });

                            if (percent <= 0)
                                return BadRequest(ErrorCodeEnum.CodeNotFound, Resource.CodeNotFound, null);

                            decimal newPrice = (100 - percent) * amount / 100;

                            // Query to calculate the sum of the amount for a user
                            // Query to calculate the sum of the amount for a user
                            string query = @"
                                           SELECT
                                               SUM(CASE WHEN action_type_id = 1 AND is_successful = true THEN amount ELSE 0 END) -
                                               SUM(CASE WHEN action_type_id = 2 AND is_successful = true THEN amount ELSE 0 END) AS balance
                                           FROM
                                               wallet_actions
                                           WHERE
                                               user_id = @UserId";

                            // Execute the query
                            var result = await dbConnection.ExecuteScalarAsync<decimal>(query, new { UserId = userId });

                            if (result < newPrice)
                                return BadRequest(ErrorCodeEnum.WalletAmountError, Resource.WalletAmountError, null);

                            string bookQuery = "SELECT book_name FROM books WHERE id = @BookId";

                            var bookName = await dbConnection.ExecuteScalarAsync<string>(bookQuery, new { BookId = model.BookId });

                            string query1 = @"
                              INSERT INTO wallet_actions (action_type_id, user_id, amount, is_successful, description, created_date)
                              VALUES (2, @UserId, @Amount, true, 'خرید کتاب ' || @BookName, CURRENT_TIMESTAMP + INTERVAL '210 minutes')";

                            string query2 = @"
                              INSERT INTO user_books (book_id, user_id, bought_time)
                              VALUES (@BookId, @UserId, CURRENT_TIMESTAMP + INTERVAL '210 minutes')";

                            string query3 = @"
                              INSERT INTO user_discounts (discount_id, user_id)
                              VALUES (@DiscountId, @UserId)";

                            string query4 = @"
                              UPDATE discounts
                              SET quantity = quantity - 1
                              WHERE id = @DiscountId";

                            // Combine both queries into a single command
                            string combinedQuery = $"{query1}; {query2}; {query3}; {query4}";

                            // Execute the combined query within the transaction
                            await dbConnection.ExecuteAsync(combinedQuery, new { UserId = userId, BookId = model.BookId, Amount = newPrice, DiscountId = model.DiscountId, BookName = bookName }, transaction);

                            transaction.Commit();  // Commit the transaction if everything is successful
                            return Ok();
                        }
                        else
                        {
                            // Query to calculate the sum of the amount for a user
                            string query = @"
                                          SELECT
                                              SUM(CASE WHEN action_type_id = 1 AND is_successful = true THEN amount ELSE 0 END) -
                                              SUM(CASE WHEN action_type_id = 2 AND is_successful = true THEN amount ELSE 0 END) AS balance
                                          FROM
                                              wallet_actions
                                          WHERE
                                              user_id = @UserId";

                            // Execute the query
                            var result = await dbConnection.ExecuteScalarAsync<decimal>(query, new { UserId = userId });

                            if (result < amount)
                                return BadRequest(ErrorCodeEnum.WalletAmountError, Resource.WalletAmountError, null);

                            string bookQuery = "SELECT book_name FROM books WHERE id = @BookId";

                            var bookName = await dbConnection.ExecuteScalarAsync<string>(bookQuery, new { BookId = model.BookId });

                            string query1 = @"
                            INSERT INTO wallet_actions (action_type_id, user_id, amount, is_successful, description, created_date)
                            VALUES (2, @UserId, @Amount, true, 'خرید کتاب ' || @BookName, CURRENT_TIMESTAMP + INTERVAL '210 minutes')";

                            // Query to add books to userBooks
                            string query2 = @"
                            INSERT INTO user_books (book_id, user_id, bought_time)
                            VALUES (@BookId, @UserId, CURRENT_TIMESTAMP + INTERVAL '210 minutes')";

                            // Combine both queries into a single command
                            string combinedQuery = $"{query1}; {query2};";

                            await dbConnection.ExecuteAsync(combinedQuery, new { UserId = userId, BookId = model.BookId, Amount = amount, BookName = bookName }, transaction);

                            transaction.Commit();  // Commit the transaction if everything is successful
                            return Ok();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();  // Roll back the transaction if an exception occurs
                        _logger.LogError(ex, null, null);
                        return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
                    }
                }
            }
        }

        private async Task<ServiceResult> WelcomeCodeCheck(string? code, int? discountId, int userId)
        {
            using (IDbConnection dbConnection = _Context.Connection)
            {
                dbConnection.Open();

                if (discountId is null)
                {
                    if (code == Resource.WelcomeDiscountCode)
                    {
                        string query1 = "SELECT 1 FROM user_books WHERE user_id = @UserId";

                        // Execute the query
                        var userInfo = await dbConnection.QueryFirstOrDefaultAsync(query1, new { UserId = userId });

                        if (userInfo is not null)
                            return BadRequest(ErrorCodeEnum.WelcomeCodeError, Resource.WelcomeCodeError, null);
                    }
                }
                else
                {
                    if (discountId == 1)
                    {
                        string query1 = "SELECT 1 FROM user_books WHERE user_id = @UserId";

                        // Execute the query
                        var userInfo = await dbConnection.QueryFirstOrDefaultAsync(query1, new { UserId = userId });

                        if (userInfo is not null)
                            return BadRequest(ErrorCodeEnum.WelcomeCodeError, Resource.WelcomeCodeError, null);
                    }
                }
                return Ok();
            }
        }
        private async Task<ServiceResult> ValidateDiscount(string? code, int? discountId, int userId)
        {
            try
            {
                using (IDbConnection dbConnection = _Context.Connection)
                {
                    dbConnection.Open();

                    var welcomecodeCheck = await WelcomeCodeCheck(code, discountId, userId);

                    if (welcomecodeCheck.Result.Http_Status_Code != (int)HttpStatusCode.OK)
                    {
                        return welcomecodeCheck;
                    }
                    else
                    {
                        if (discountId is null)
                        {
                            // Query to get discount information
                            string query = "SELECT id, quantity, expire_date, percent FROM discounts WHERE code = @Code";

                            // Execute the query
                            var discountInfo = await dbConnection.QueryFirstOrDefaultAsync(query, new { Code = code });

                            if (discountInfo == null)
                                return BadRequest(ErrorCodeEnum.CodeNotFound, Resource.CodeNotFound, null);

                            // Check quantity
                            if (discountInfo.quantity == 0)
                                return BadRequest(ErrorCodeEnum.CodeFinished, Resource.CodeFinished, null);

                            // Check expiration date
                            if (discountInfo.expiredate < DateTime.Today)
                                return BadRequest(ErrorCodeEnum.CodeExpired, Resource.CodeExpired, null);

                            // Check if the discount is already used
                            var isUsedQuery = "SELECT 1 FROM user_discounts WHERE discount_id = @DiscountId AND user_id = @UserId";
                            var isUsed = await dbConnection.ExecuteScalarAsync<int?>(isUsedQuery, new { DiscountId = discountInfo.id, UserId = userId });

                            if (isUsed != null)
                                return BadRequest(ErrorCodeEnum.CodeUsed, Resource.CodeUsed, null);

                            return null; // No validation errors}
                        }
                        else
                        {
                            // Query to get discount information
                            string query = "SELECT id, quantity, expire_date, percent FROM discounts WHERE id = @DiscountId";

                            // Execute the query
                            var discountInfo = await dbConnection.QueryFirstOrDefaultAsync(query, new { DiscountId = discountId });

                            if (discountInfo == null)
                                return BadRequest(ErrorCodeEnum.CodeNotFound, Resource.CodeNotFound, null);

                            // Check quantity
                            if (discountInfo.quantity == 0)
                                return BadRequest(ErrorCodeEnum.CodeFinished, Resource.CodeFinished, null);

                            // Check expiration date
                            if (discountInfo.expiredate < DateTime.Today)
                                return BadRequest(ErrorCodeEnum.CodeExpired, Resource.CodeExpired, null);

                            // Check if the discount is already used
                            var isUsedQuery = "SELECT 1 FROM user_discounts WHERE discount_id = @DiscountId AND user_id = @userId";
                            var isUsed = await dbConnection.ExecuteScalarAsync<int?>(isUsedQuery, new { DiscountId = discountInfo.id, UserId = userId });

                            if (isUsed != null)
                                return BadRequest(ErrorCodeEnum.CodeUsed, Resource.CodeUsed, null);

                            return null; // No validation errors}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }

        public async Task<ServiceResult> UserBookmarked(int userId, int bookId)
        {
            try
            {
                using (IDbConnection dbConnection = _Context.Connection)
                {
                    dbConnection.Open();

                    // Query to check if the user has purchased the book
                    string query = "SELECT 1 FROM user_bookmarks WHERE user_id = @UserId AND book_id = @BookId AND is_delete = false";

                    // Execute the query
                    var result = await dbConnection.ExecuteScalarAsync<int?>(query, new { UserId = userId, BookId = bookId });

                    // Check the result and return accordingly
                    if (result != null)
                    {
                        return Ok(true); // The user has purchased the book
                    }
                    else
                    {
                        return Ok(false); // The user has not purchased the book
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }
    }
}
