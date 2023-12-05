using Entities.Base;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class ValidateAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly int roleId;

    public ValidateAuthorizationAttribute(int roleId)
    {
        this.roleId = roleId;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
        string userId;

        try
        {
            IActionResult validationError = AuthorizationHelper.ValidateAuthorization(authorizationHeader, roleId, out userId);

            if (validationError != null)
            {
                context.Result = validationError;
                return;
            }

            // Add userId to the HttpContext so that it can be accessed in the controller
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("user_id", userId)
            }, "jwt"));
        }
        catch (Exception ex)
        {
            // Handle unexpected exceptions
            context.Result = new ObjectResult(new ServiceResult(null, new ApiResult(HttpStatusCode.Unauthorized, ErrorCodeEnum.InternalError, ex.Message, null)))
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }
    }
}
