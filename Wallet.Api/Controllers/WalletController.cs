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
using Microsoft.AspNetCore.Http.HttpResults;
using System.IdentityModel.Tokens.Jwt;
using Wallet.Common.Utilities;
using Wallet.Core.Interfaces;

namespace Wallet.Api.Controllers
{
    public class WalletController : APIControllerBase
    {
        private readonly ILogger<WalletController> _logger;
        private readonly ISender _sender;
        private readonly IWalletService _service;
        public WalletController(IWalletService service, ILogger<WalletController> logger, ISender sender)
        {
            _logger = logger;
            _service = service;
            _sender = sender;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChargeWallet([FromHeader(Name = "Bearer")] string authorizationHeader, [FromQuery] int amount)
        {
            try
            {
                if (string.IsNullOrEmpty(authorizationHeader))
                    return BadRequest(Resource.AuthorizationHeaderMissing);

                // Authorization header typically has the format "Bearer {token}"
                // Extract the token portion
                var jwtToken = authorizationHeader?.Split(" ").LastOrDefault();

                if(jwtToken is null)
                    return BadRequest(Resource.UserIdClaimMissing);

                var userId = JwtTokenHelper.GetUserIdByClaim(jwtToken);

                if (userId is null)
                    return BadRequest(Resource.UserIdClaimMissing);

                if (amount <= 0)
                    return BadRequest(Resource.AmountError);

                var model = new ChargeWalletViewModel
                {
                    amount = amount,
                    userId = int.Parse(userId)
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
