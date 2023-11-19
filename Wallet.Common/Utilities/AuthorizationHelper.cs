using Entities.Base;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Wallet.Common.Resources;
using Wallet.Common.Utilities;

public static class AuthorizationHelper
{
    public static IActionResult ValidateAuthorization(string authorizationHeader, int roleId, out string userId)
    {
        userId = null;

        if (string.IsNullOrEmpty(authorizationHeader))
            return Unauthorized(ErrorCodeEnum.BadGateway, Resource.AuthorizationHeaderMissing);

        // Extract the token portion from the authorization header
        var jwtToken = authorizationHeader.Split(" ").LastOrDefault();

        if (jwtToken is null)
            return Unauthorized(ErrorCodeEnum.BadGateway, Resource.UserIdClaimMissing);

        userId = JwtTokenHelper.GetUserIdByClaim(jwtToken);
        string roleIdOfUser = JwtTokenHelper.GetUserRoleIdByClaim(jwtToken);
        DateTime? expTime = JwtTokenHelper.GetExpirationTime(jwtToken);

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
            return Unauthorized(ErrorCodeEnum.PermissionDenied, Resource.RoleDoesNotMatchUser);

        return null; // Indicates successful validation 
    }

    private static IActionResult Unauthorized(ErrorCodeEnum errorCode, string errorMessage)
    {
        // You can customize this method based on your API response structure
        // For simplicity, I'm returning a BadRequest with an error message.
        return new UnauthorizedObjectResult(new ServiceResult(null, new ApiResult(HttpStatusCode.Unauthorized, errorCode, errorMessage, null)));
    }
}
