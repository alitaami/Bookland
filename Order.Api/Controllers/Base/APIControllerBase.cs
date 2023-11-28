using Microsoft.AspNetCore.Mvc;
using System.Net;
using Entities.Base;


[ApiController]
public class APIControllerBase : ControllerBase
{
    protected IActionResult APIResponse(ServiceResult serviceResult)
    {
        if (serviceResult.Result.Http_Status_Code == (int)HttpStatusCode.OK)
        {
            if (serviceResult.Data == null)
                return Ok();
            else
                return Ok(serviceResult.Data);
        }

        else if (serviceResult.Result.Http_Status_Code == (int)HttpStatusCode.BadRequest)
            return BadRequest(serviceResult.Result);

        else if (serviceResult.Result.Http_Status_Code == (int)HttpStatusCode.NotFound)
            return NotFound(serviceResult.Result);

        else if (serviceResult.Result.Http_Status_Code == (int)HttpStatusCode.InternalServerError)
            return StatusCode((int)HttpStatusCode.InternalServerError, serviceResult.Data);

        else //TODO : این مورد بررسی بشه شاید نیاز به تغییر باشه
            return StatusCode((int)HttpStatusCode.InternalServerError);
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
    protected IActionResult BadRequestError(ErrorCodeEnum error, string message)
    {
        return APIResponse(new ServiceResult(null, CreateBadRequestErrorResult(error, message)));
    }

    private ApiResult CreateInternalErrorResult(ErrorCodeEnum error, string? message)
    {
        return new ApiResult(HttpStatusCode.InternalServerError, error, message, null);
    } 
    private ApiResult CreateBadRequestErrorResult(ErrorCodeEnum error, string? message)
    {
        return new ApiResult(HttpStatusCode.BadRequest, error, message, null);
    } 
}
