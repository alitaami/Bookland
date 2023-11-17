using Entities.Base;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Net;
using Wallet.Common.Resources;
using MediatR;
using System.Reflection;
using Wallet.Application.Features.Commands;
using Wallet.Core.ViewModels;
using System.Security.Claims;

namespace Wallet.Api.Controllers
{
    public class WalletController : APIControllerBase
    {
        private readonly ILogger<WalletController> _logger;
        private ISender _sender;

        public WalletController(ILogger<WalletController> logger, ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChargeWallet([FromQuery] int amount)
        {
            try
            {
                if (amount.Equals(0))
                    return InternalServerError(Resource.AmountError);

                //var userId = User.FindFirstValue("user-id");

                //if (string.IsNullOrEmpty(userId))
                //    return InternalServerError(Resource.GeneralErrorTryAgain);

                var model = new ChargeWalletViewModel
                {

                    amount = amount,
                    //TODO : user-id is string we should cast that
                    userId = 1
                };

                var res = await _sender.Send(new ChargeWalletCommand(model));

                return APIResponse(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message);
            }
        }
    }
}
