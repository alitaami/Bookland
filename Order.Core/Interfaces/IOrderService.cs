using Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Core.Interfaces
{
    public interface IOrderService
    {
        public Task<ServiceResult> UserPurchased (int userId,int bookId);
        public Task<ServiceResult> AdjustDiscount (int userId, string code, decimal amount);
        public Task<ServiceResult> PurchaseBook (int userId, int bookId, decimal amount, int discountId);
    }
}
