using Dapper;
using Entities.Base;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Features.Commands;
using Wallet.Common.Resources;
using Wallet.Core.Interfaces;

namespace Wallet.Infrastructure.Services
{
    public class WalletService : ServiceBase<WalletService>, IWalletService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _Context;

        public WalletService(IConfiguration configuration, ApplicationDbContext dbContext, ILogger<WalletService> logger) : base(logger)
        {
            _configuration = configuration;
            _Context = dbContext;
        }
        public async Task<ServiceResult> ChargeWallet(int id, int amount)
        {
            try
            {
                int walletActionId = 0;
                 
                IEnumerable<dynamic> data = Enumerable.Empty<dynamic>();

                string insertQuery = @"
                                      INSERT INTO wallet_actions (action_type_id, user_id, amount, is_successful, description, created_date)
                                      VALUES (@ActionTypeId, @UserId, @Amount, @IsSuccessful, @Description, @CreatedDate)
                                      RETURNING id;
                                      ";
                 
                var parameters = new
                {
                    ActionTypeId = 1,
                    UserId = id,
                    Amount = amount,
                    IsSuccessful = false, // You may need to determine the success based on your logic
                    Description = $" {amount} تومان  واریز به حساب", // You may want to adjust the description
                    CreatedDate = DateTime.Now  // You may want to adjust the creation date
                };

                using (IDbConnection dbConnection = _Context.Connection)
                {
                    dbConnection.Open();
                    // TODO: Charging Logic 
                    walletActionId = await dbConnection.ExecuteScalarAsync<int>(insertQuery, parameters);
                }

                return Ok(walletActionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        } 
        public async Task<ServiceResult> UpdateWallet(int walletActionId)
        {
            try
            {
                string updateQuery = @"
                                     UPDATE wallet_actions
                                     SET is_successful = true
                                     WHERE id = @WalletActionId;";

                var parameters = new
                {
                    WalletActionId = walletActionId
                };

                using (IDbConnection dbConnection = _Context.Connection)
                {
                    dbConnection.Open();
                    await dbConnection.ExecuteAsync(updateQuery, parameters);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }
        
    }
}
