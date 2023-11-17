using Dapper;
using Entities.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                 IEnumerable<dynamic> data = Enumerable.Empty<dynamic>();

                using (IDbConnection dbConnection = _Context.Connection)
                {
                    dbConnection.Open();
                    data = await dbConnection.QueryAsync("SELECT * FROM walletactions");
                }

                //if (res.IsFaulted)
                //    return BadRequest(ErrorCodeEnum.BadRequest, Resource.CreateError, null);///

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }
    }
}
