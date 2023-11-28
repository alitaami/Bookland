using Dapper;
using Entities.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using Npgsql;
using Order.Common.Resources;
using Order.Core.Interfaces;
using System.Data;

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

        public async Task<ServiceResult> AdjustDiscount(int userId, decimal bookPrice, string code)
        {
            try
            {
                using (IDbConnection dbConnection = _Context.Connection)
                {
                    dbConnection.Open();

                    // Query to get discount information
                    string query = "SELECT quantity, expiredate, percent FROM public.discounts WHERE code = @Code";

                    // Execute the query
                    var discountInfo = await dbConnection.QueryFirstOrDefaultAsync(query, new { Code = code });

                    if (discountInfo == null)
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.CodeNotFound, null);

                    // Check quantity
                    if (discountInfo.quantity == 0)
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.CodeFinished, null);

                    // Check expiration date
                    if (discountInfo.expiredate < DateTime.Today)
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.CodeExpired, null);

                    // Check if the discount is already used
                    var isUsedQuery = "SELECT 1 FROM public.userdiscounts WHERE discountid = @DiscountId AND UserId = @userId";
                    var isUsed = await dbConnection.ExecuteScalarAsync<int?>(isUsedQuery, new { DiscountId = discountInfo.id, UserId = userId });

                    if (isUsed != null)
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.CodeUsed, null);

                    // Calculate new price
                    var newPrice = bookPrice * (100 - discountInfo.percent);

                    return Ok(newPrice);
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
                    string query = "SELECT 1 FROM public.userbooks WHERE userid = @UserId AND bookid = @BookId";

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
         
        public async Task<ServiceResult> PurchaseBook(int userId, int bookId, decimal newBookPrice)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }

    }
}
