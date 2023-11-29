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
    public class CheckBookQuery : IRequest<ServiceResult>
    {
        public int BookId { get; set; }
        public int UserId { get; set; }

        public CheckBookQuery(int bookId, int userId)
        {
            BookId = bookId;
            UserId = userId;
        }
    }
    public class CheckBookQueryHandler : ServiceBase<CheckBookQueryHandler>, IRequestHandler<CheckBookQuery, ServiceResult>
    {
        private readonly IOrderService __order;
        public CheckBookQueryHandler(ILogger<CheckBookQueryHandler> logger, IOrderService order) : base(logger)
        {
            __order = order;
        }

        public async Task<ServiceResult> Handle(CheckBookQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var res = await __order.UserPurchased(request.UserId,request.BookId);

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
