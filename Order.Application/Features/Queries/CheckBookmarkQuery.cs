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
    public class CheckBookmarkQuery : IRequest<ServiceResult>
    {
        public int BookId { get; set; }
        public int UserId { get; set; }

        public CheckBookmarkQuery(int bookId, int userId)
        {
            BookId = bookId;
            UserId = userId;
        }
    }
    public class CheckBookmarkQueryHandler : ServiceBase<CheckBookmarkQueryHandler>, IRequestHandler<CheckBookmarkQuery, ServiceResult>
    {
        private readonly IOrderService __order;
        public CheckBookmarkQueryHandler(ILogger<CheckBookmarkQueryHandler> logger, IOrderService order) : base(logger)
        {
            __order = order;
        }

        public async Task<ServiceResult> Handle(CheckBookmarkQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var res = await __order.UserBookmarked(request.UserId,request.BookId);

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
