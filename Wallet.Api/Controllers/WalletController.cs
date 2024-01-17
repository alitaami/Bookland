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
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.Graph.Models;
using Wallet.Common.Utilities.Authorization;
using Microsoft.AspNetCore.Cors;

namespace Wallet.Api.Controllers
{
    public class WalletController : APIControllerBase
    {
        private readonly ILogger<WalletController> _logger;
        private readonly ISender _sender;
        private readonly IWalletService _service;
        private readonly IMapper _mapper;
        public WalletController(IMapper mapper, IWalletService service, ILogger<WalletController> logger, ISender sender)
        {
            _mapper = mapper;
            _logger = logger;
            _service = service;
            _sender = sender;
        }

        #region UserWallet
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Route("api/user/wallet/[action]")]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [ValidateAuthorization(2)] // Specify the required roleId
        public async Task<IActionResult> ChargeUserWallet([FromHeader(Name = "Authorization")] string authorizationHeader, [FromQuery] int amount)
        {
            try
            {
                // Use HttpContext.User.Claims to retrieve user claims
                string userId = HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;

                if (amount > 200000 || amount < 5000)
                    return InternalServerError(ErrorCodeEnum.AmountError, Resource.AmountError);

                var model = new ChargeWalletViewModel
                {
                    amount = amount,
                    userId = int.Parse(userId)
                };

                var walletAction_Id = await _sender.Send(new ChargeWalletCommand(model));

                int walletActionId = _mapper.Map<int>(walletAction_Id.Data);

                #region ZarinPal Implementation
                var payment = new ZarinpalSandbox.Payment(amount);
                var res = payment.PaymentRequest("شارژ کیف پول", "http://localhost:3000/user/wallet/");

                if (res.Result.Status == 100)
                {
                    string authority = res.Result.Authority; // Remove leading zeros
                    string baseUrl = "https://sandbox.zarinpal.com/pg/StartPay/";

                    string redirectUrl = $"{baseUrl}{authority}?walletActionId={walletActionId}";

                    ServiceResult result = new ServiceResult(redirectUrl, new ApiResult(HttpStatusCode.OK, ErrorCodeEnum.None, "", null));

                    return APIResponse(result);
                }
                else
                {
                    return InternalServerError(ErrorCodeEnum.DepositError, Resource.DepositFail);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message);
            }
        }

        [Route("api/user/wallet/[action]")]
        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [ValidateAuthorization(2)] // Specify the required roleId
        public async Task<IActionResult> UpdateUserWallet([FromHeader(Name = "Authorization")] string authorizationHeader, [FromQuery] int walletActionId)
        {
            try
            {
                var res = await _sender.Send(new UpdateWalletCommand(walletActionId));

                return APIResponse(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message);
            }
        }
        #endregion
        #region Publisher Wallet
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Route("api/publisher/wallet/[action]")]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [ValidateAuthorization(1)] // Specify the required roleId
        public async Task<IActionResult> ChargePublisherWallet([FromHeader(Name = "Authorization")] string authorizationHeader, [FromQuery] int amount)
        {
            try
            {
                // Use HttpContext.User.Claims to retrieve user claims
                string userId = HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;

                if (amount > 200000 || amount < 5000)
                    return InternalServerError(ErrorCodeEnum.AmountError, Resource.AmountError);

                var model = new ChargeWalletViewModel
                {
                    amount = amount,
                    userId = int.Parse(userId)
                };

                var walletAction_Id = await _sender.Send(new ChargeWalletCommand(model));

                int walletActionId = _mapper.Map<int>(walletAction_Id.Data);

                #region ZarinPal Implementation
                var payment = new ZarinpalSandbox.Payment(amount);
                var res = payment.PaymentRequest("شارژ کیف پول", "http://localhost:3000/Publisher/views/ChargeWallet");

                if (res.Result.Status == 100)
                {
                    string authority = res.Result.Authority; // Remove leading zeros
                    string baseUrl = "https://sandbox.zarinpal.com/pg/StartPay/";

                    string redirectUrl = $"{baseUrl}{authority}?walletActionId={walletActionId}";

                    ServiceResult result = new ServiceResult(redirectUrl, new ApiResult(HttpStatusCode.OK, ErrorCodeEnum.None, "", null));

                    return APIResponse(result);
                }
                else
                {
                    return InternalServerError(ErrorCodeEnum.DepositError, Resource.DepositFail);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message);
            }
        }

        [Route("api/publisher/wallet/[action]")]
        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [ValidateAuthorization(1)] // Specify the required roleId
        public async Task<IActionResult> UpdatePublisherWallet([FromHeader(Name = "Authorization")] string authorizationHeader, [FromQuery] int walletActionId)
        {
            try
            {
                var res = await _sender.Send(new UpdateWalletCommand(walletActionId));

                return APIResponse(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message);
            }
        }
        #endregion
    }
}
