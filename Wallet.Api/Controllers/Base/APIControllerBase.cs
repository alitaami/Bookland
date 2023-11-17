using Microsoft.AspNetCore.Mvc;
using System.Net;
using Entities.Base;
using Wallet.Common.Resources;

//[Route("api/[controller]/[action]")]
[ApiController]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]  // api/v1/[controller]
    public class APIControllerBase : ControllerBase
    {
        protected IActionResult APIResponse(ServiceResult serviceResult)
        {
            if (serviceResult.Result.HttpStatusCode == (int)HttpStatusCode.OK)
            {
                if (serviceResult.Data == null)
                    return Ok();
                else
                    return Ok(serviceResult.Data);
            }

            else if (serviceResult.Result.HttpStatusCode == (int)HttpStatusCode.BadRequest)
                return BadRequest(serviceResult.Result);

            else if (serviceResult.Result.HttpStatusCode == (int)HttpStatusCode.NotFound)
                return NotFound(serviceResult.Result);

            else if (serviceResult.Result.HttpStatusCode == (int)HttpStatusCode.InternalServerError)
                return StatusCode((int)HttpStatusCode.InternalServerError, serviceResult.Data);

            else //TODO : این مورد بررسی بشه شاید نیاز به تغییر باشه
                return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        protected IActionResult InternalServerError()
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiResult(HttpStatusCode.InternalServerError,
                    ErrorCodeEnum.InternalError,
                    Resource.GeneralErrorTryAgain
                    , null));
        }

        protected IActionResult InternalServerError(ErrorCodeEnum error)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiResult(HttpStatusCode.InternalServerError,
                    error,
                    Resource.GeneralErrorTryAgain
                    , null));
        }

        protected IActionResult InternalServerError(string message)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiResult(HttpStatusCode.InternalServerError,
                    ErrorCodeEnum.InternalError,
                    message
                    , null));
        }

        protected IActionResult InternalServerError(ErrorCodeEnum error, string message)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiResult(HttpStatusCode.InternalServerError,
                    error,
                    message
                    , null));
        }
    }
 