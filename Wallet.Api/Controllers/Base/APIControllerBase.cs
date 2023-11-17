using Microsoft.AspNetCore.Mvc;
using System.Net;
using Entities.Base;
using Wallet.Common.Resources;
using Tavis.UriTemplates;

[Route("api/[controller]/[action]")]
[ApiController]
public class APIControllerBase : ControllerBase
{
    protected IActionResult APIResponse(ServiceResult serviceResult)
    {
        if (serviceResult.Result.Http_Status_Code == (int)HttpStatusCode.OK)
        {
            if (serviceResult.Data == null)
            {
                return Ok();
            }
            else
            {
                return Ok(serviceResult.Data);
            }
        }
        else if (serviceResult.Result.Http_Status_Code == (int)HttpStatusCode.BadRequest)
        {
            return BadRequest(serviceResult.Result);
        }
        else if (serviceResult.Result.Http_Status_Code == (int)HttpStatusCode.NotFound)
        {
            return NotFound(serviceResult.Result);
        }
        else if (serviceResult.Result.Http_Status_Code == (int)HttpStatusCode.InternalServerError)
        {
            // Return a new ServiceResult with null data for internal server errors
            return StatusCode((int)HttpStatusCode.InternalServerError, new ServiceResult(null, serviceResult.Result));
        }
        else
        {
            // Handle other cases, if needed
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }


    protected IActionResult InternalServerError()
    {
        return APIResponse(new ServiceResult(null, CreateInternalErrorResult(ErrorCodeEnum.InternalError, null)));
    }

    protected IActionResult InternalServerError(ErrorCodeEnum error)
    {
        return APIResponse(new ServiceResult(null, CreateInternalErrorResult(error, null)));
    }

    protected IActionResult InternalServerError(string message)
    {
        return APIResponse(new ServiceResult(null, CreateInternalErrorResult(ErrorCodeEnum.InternalError, message)));
    }

    protected IActionResult InternalServerError(ErrorCodeEnum error, string message)
    {
        return APIResponse(new ServiceResult(null, CreateInternalErrorResult(error, message)));
    }

    private ApiResult CreateInternalErrorResult(ErrorCodeEnum error, string? message)
    {
        return new ApiResult(HttpStatusCode.InternalServerError, error, message, null);
    }
}
