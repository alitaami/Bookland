using Entities.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Application.Models.ViewModels;
using Order.Common.Resources;
using Order.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Commands
{
    public class PurchaseBookCommand : IRequest<ServiceResult>
    {
        public int UserId { get; set; }
        public PurchaseBookViewModel Model { get; set; }

        public PurchaseBookCommand( int userId,PurchaseBookViewModel model)
        {
            UserId = userId;
            Model = model;
        }
    }
    public class PurchaseBookCommandHandler : ServiceBase<PurchaseBookCommandHandler>, IRequestHandler<PurchaseBookCommand, ServiceResult>
    {
        private readonly IOrderService _Order;
        public PurchaseBookCommandHandler(ILogger<PurchaseBookCommandHandler> logger, IOrderService _order) : base(logger)
        {
            _Order = _order;
        }

        public async Task<ServiceResult> Handle(PurchaseBookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var res = await _Order.PurchaseBook(request.UserId, request.Model);
    
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
