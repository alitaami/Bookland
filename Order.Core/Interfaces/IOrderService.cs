using Entities.Base;
using Order.Application.Models.ViewModels;

namespace Order.Core.Interfaces
{
    public interface IOrderService
    {
        public Task<ServiceResult> UserPurchased (int userId,int bookId);
        public Task<ServiceResult> AdjustDiscount (int userId, string code, decimal amount);
        public Task<ServiceResult> PurchaseBook (int userId, PurchaseBookViewModel model);
    }
}
