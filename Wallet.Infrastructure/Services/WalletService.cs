using Entities.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.Interfaces;

namespace Wallet.Infrastructure.Services
{
    public class WalletService : ServiceBase<WalletService>, IWalletService
    {
        public WalletService(ILogger<WalletService> logger) : base (logger)
        {

        }
        public Task<ServiceResult> ChargeWallet(int amount)
        {
            throw new NotImplementedException();
        }
    }
}
