using Entities.Base;
using Microsoft.AspNetCore.Mvc;
using Order.Common.Resources;
using System.Net;
using Wallet.Common.Utilities.Authorization;

public static class AuthorizationHelper
{
    public static IActionResult ValidateAuthorization(string authorizationHeader, int roleId, out string userId)
    {
        userId = null;

        if (string.IsNullOrEmpty(authorizationHeader))
            return Unauthorized(ErrorCodeEnum.BadGateway, Resource.AuthorizationHeaderMissing);

        string[] jwtToken = authorizationHeader.Split(" ");

        if (jwtToken.Length != 2 || jwtToken[0] != "Bearer" || jwtToken[1] is null)
            return Unauthorized(ErrorCodeEnum.BadGateway, Resource.TokenTypeError);

        userId = JwtTokenHelper.GetUserIdByClaim(jwtToken[1]);
        string roleIdOfUser = JwtTokenHelper.GetUserRoleIdByClaim(jwtToken[1]);
        DateTime? expTime = JwtTokenHelper.GetExpirationTime(jwtToken[1]);

        if (!expTime.HasValue)
            return Unauthorized(ErrorCodeEnum.JwtTimeIsNull, Resource.JwtExpTimeClaimMissing);

        if (expTime.Value < DateTime.UtcNow)
            return Unauthorized(ErrorCodeEnum.JwtExpired, Resource.JwtTokenExpired);

        if (roleIdOfUser is null)
            return Unauthorized(ErrorCodeEnum.BadGateway, Resource.RoleIdClaimMissing);

        if (userId is null)
            return Unauthorized(ErrorCodeEnum.BadGateway, Resource.UserIdClaimMissing);

        int role_Id = int.Parse(roleIdOfUser);

        if (role_Id != roleId)
            return Forbidden(ErrorCodeEnum.PermissionDenied, Resource.RoleDoesNotMatchUser);

        return null; // Indicates successful validation 
    }

    private static IActionResult Unauthorized(ErrorCodeEnum errorCode, string errorMessage)
    {
        // You can customize this method based on your API response structure
        // For simplicity, I'm returning a BadRequest with an error message.
        return new UnauthorizedObjectResult(new ServiceResult(null, new ApiResult(HttpStatusCode.Unauthorized, errorCode, errorMessage, null)));
    }
    private static IActionResult Forbidden(ErrorCodeEnum errorCode, string errorMessage)
    {
        return new ObjectResult(new ServiceResult(null, new ApiResult(HttpStatusCode.Forbidden, errorCode, errorMessage, null)))
        {
            StatusCode = (int)HttpStatusCode.Forbidden
        };
    }

}
