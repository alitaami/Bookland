using Entities.Base;
using MediatR;
using Microsoft.Extensions.Logging;
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

        public CheckBookQuery(int bookId)
        {
            BookId = bookId;
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
                var res = await __order.PurchaseBook(request.BookId, cancellationToken);

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
