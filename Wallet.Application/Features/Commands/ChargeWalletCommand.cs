using Entities.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Features.Commands;
using Wallet.Core.Interfaces;
using Wallet.Core.ViewModels;

namespace Wallet.Application.Features.Commands
{
    public class ChargeWalletCommand : IRequest<ServiceResult>
    {
        public ChargeWalletViewModel model { get; set; }

        public ChargeWalletCommand(ChargeWalletViewModel walletViewModel)
        {
            model = walletViewModel;
        }
    }
    public class ChargeWalletCommandHandler : ServiceBase<ChargeWalletCommandHandler>, IRequestHandler<ChargeWalletCommand, ServiceResult>
    {
        private readonly IWalletService _service;
        public ChargeWalletCommandHandler(IWalletService service, ILogger<ChargeWalletCommandHandler> logger) : base(logger)
        {
            _service = service;
        }
        public async Task<ServiceResult> Handle(ChargeWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var res = _service.ChargeWallet(request.model.userId, request.model.amount);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }
    }
}
