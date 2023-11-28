using Entities.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Common.Resources;
using Order.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Queries
{ 
    public class AdjustDiscountQuery : IRequest<ServiceResult>
    {
        public string Code { get; set; }
        public decimal Amount { get; set; } 
        public int UserId { get; set; }

        public AdjustDiscountQuery(int userId,string code, decimal amount)
        {
            UserId = userId;
            Code = code;
            Amount = amount;
        }
    }
    public class AdjustDiscountQueryHandler : ServiceBase<AdjustDiscountQueryHandler>, IRequestHandler<AdjustDiscountQuery, ServiceResult>
    {
        private readonly IOrderService __order;
        public AdjustDiscountQueryHandler(ILogger<AdjustDiscountQueryHandler> logger, IOrderService order) : base(logger)
        {
            __order = order;
        } 
        public async Task<ServiceResult> Handle(AdjustDiscountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var res = await __order.AdjustDiscount( request.UserId,request.Code,request.Amount );

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
    }
}
