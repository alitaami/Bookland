using Entities.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
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

        public async Task<ServiceResult> AdjustDiscount(decimal bookPrice, string code)
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

        public async Task<ServiceResult> CheckBook(int userId, int bookId)
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
        //public async Task<ServiceResult> ChargeWallet(int id, int amount)
        //{
        //    try
        //    {
        //        int walletActionId = 0;

        //        IEnumerable<dynamic> data = Enumerable.Empty<dynamic>();

        //        string insertQuery = @"
        //        INSERT INTO WalletActions (ActionTypeId, UserId, Amount, IsSuccessful, Description, CreatedDate)
        //        VALUES (@ActionTypeId, @UserId, @Amount, @IsSuccessful, @Description, @CreatedDate)
        //        RETURNING Id;
        //        ";

        //        var parameters = new
        //        {
        //            ActionTypeId = 1,
        //            UserId = id,
        //            Amount = amount,
        //            IsSuccessful = false, // You may need to determine the success based on your logic
        //            Description = $" {amount} تومان  واریز به حساب", // You may want to adjust the description
        //            CreatedDate = DateTime.Now.AddHours(3.5) // You may want to adjust the creation date
        //        };

        //        using (IDbConnection dbConnection = _Context.Connection)
        //        {
        //            dbConnection.Open();
        //            // TODO: Charging Logic 
        //            walletActionId = await dbConnection.ExecuteScalarAsync<int>(insertQuery, parameters);
        //        }

        //        return Ok(walletActionId);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, null, null);

        //        return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
        //    }
        //} 
        //public async Task<ServiceResult> UpdateWallet(int walletActionId)
        //{
        //    try
        //    {
        //        string updateQuery = @"
        //        UPDATE WalletActions
        //        SET IsSuccessful = true
        //        WHERE Id = @WalletActionId;";

        //        var parameters = new
        //        {
        //            WalletActionId = walletActionId
        //        };

        //        using (IDbConnection dbConnection = _Context.Connection)
        //        {
        //            dbConnection.Open();
        //            await dbConnection.ExecuteAsync(updateQuery, parameters);
        //        }

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, null, null);

        //        return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
        //    }
        //}

    }
}
