using Entities.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.Interfaces;
using Wallet.Core.ViewModels;

namespace Wallet.Application.Features.Commands
{
    public class UpdateWalletCommand : IRequest<ServiceResult>
    {
        public int WalletActionId { get; set; }
        public UpdateWalletCommand(int walletActionId)
        {
            WalletActionId = walletActionId;
        }
    }
    public class UpdateWalletCommandHandler : ServiceBase<UpdateWalletCommandHandler>, IRequestHandler<UpdateWalletCommand, ServiceResult>
    {
        private readonly IWalletService _service;

        public UpdateWalletCommandHandler(IWalletService service, ILogger<UpdateWalletCommandHandler> logger) : base(logger)
        {
            _service = service;
        }
        public async Task<ServiceResult> Handle(UpdateWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var res = await _service.UpdateWallet(request.WalletActionId);

                // Ensure that res is a ServiceResult with the expected structure
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message, null);
            }
        }
    }
}
