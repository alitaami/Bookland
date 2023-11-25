using Entities.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wallet.Core.Interfaces;
using Wallet.Core.ViewModels;

namespace Wallet.Application.Features.Commands
{
    public class ChargeWalletCommand : IRequest<ServiceResult>
    {
        public ChargeWalletViewModel Model { get; set; }

        public ChargeWalletCommand(ChargeWalletViewModel walletViewModel)
        {
            Model = walletViewModel;
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
                    var res = await _service.ChargeWallet(request.Model.userId, request.Model.amount);

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
