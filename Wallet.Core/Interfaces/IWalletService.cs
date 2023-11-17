using Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Core.Interfaces
{
    public interface IWalletService
    {
        public Task<ServiceResult> ChargeWallet(int id,int amount);
    }
}
