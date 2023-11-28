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
        public Task<ServiceResult> CheckBook (int userId,int bookId);
        public Task<ServiceResult> AdjustDiscount (decimal bookPrice,string code);
        public Task<ServiceResult> PurchaseBook (int userId, int bookId,decimal newBookPrice);
    }
}
