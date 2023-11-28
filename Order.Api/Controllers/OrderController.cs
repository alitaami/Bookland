using Entities.Base;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Net;
using MediatR;

namespace Wallet.Api.Controllers
{
    public class OrderController : APIControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly ISender _sender;

        public OrderController(ILogger<OrderController> logger, ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Route("api/user/check-book/{bookId}")]
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [ValidateAuthorization(2)] // Specify the required roleId
        public async Task<IActionResult> CheckBook([FromHeader(Name = "Authorization")] string authorizationHeader, [FromRoute] int bookId)
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
    }
}
