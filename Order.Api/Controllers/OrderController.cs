using Entities.Base;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Net;
using MediatR;
using Order.Application.Features.Queries;
using Microsoft.IdentityModel.Tokens;
using Order.Common.Resources;
using Order.Application.Models.ViewModels;
using Order.Application.Features.Commands;
using Microsoft.AspNetCore.Cors;

namespace Order.Controllers
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
                string user_Id = HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;

                int userId = !user_Id.IsNullOrEmpty() ? int.Parse(user_Id) : 0;

                if (userId.Equals(0))
                    return BadRequestError(ErrorCodeEnum.BadRequest, Resource.TokenTypeError);

                var res = await _sender.Send(new CheckBookQuery(bookId, userId));

                return APIResponse(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message);
            }
        }
        
        [Route("api/user/check-bookmark/{bookId}")]
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [ValidateAuthorization(2)] // Specify the required roleId
        public async Task<IActionResult> CheckBookmark([FromHeader(Name = "Authorization")] string authorizationHeader, [FromRoute] int bookId)
        {
            try
            {
                // Use HttpContext.User.Claims to retrieve user claims
                string user_Id = HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;

                int userId = !user_Id.IsNullOrEmpty() ? int.Parse(user_Id) : 0;

                if (userId.Equals(0))
                    return BadRequestError(ErrorCodeEnum.BadRequest, Resource.TokenTypeError);

                var res = await _sender.Send(new CheckBookmarkQuery(bookId, userId));

                return APIResponse(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message);
            }
        }
       
        [Route("api/user/adjust-discount")]
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [ValidateAuthorization(2)] // Specify the required roleId
        public async Task<IActionResult> AdjustDiscount([FromHeader(Name = "Authorization")] string authorizationHeader, [FromQuery] string code, [FromQuery] decimal amount)
        {
            try
            {
                // Use HttpContext.User.Claims to retrieve user claims
                string user_Id = HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;

                int userId = !user_Id.IsNullOrEmpty() ? int.Parse(user_Id) : 0;

                if (userId.Equals(0))
                    return BadRequestError(ErrorCodeEnum.BadRequest, Resource.TokenTypeError);

                var res = await _sender.Send(new AdjustDiscountQuery(userId, code, amount));

                return APIResponse(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, ex.Message);
            }
        }

        [Route("api/user/purchase-book")]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [ValidateAuthorization(2)] // Specify the required roleId
        public async Task<IActionResult> PurchaseBook([FromHeader(Name = "Authorization")] string authorizationHeader, [FromBody] PurchaseBookViewModel model)
        {
            try
            {
                // Use HttpContext.User.Claims to retrieve user claims
                string user_Id = HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;

                int userId = !user_Id.IsNullOrEmpty() ? int.Parse(user_Id) : 0;

                if (userId.Equals(0))
                    return BadRequestError(ErrorCodeEnum.BadRequest, Resource.TokenTypeError);

                var res = await _sender.Send(new PurchaseBookCommand(userId, model));

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