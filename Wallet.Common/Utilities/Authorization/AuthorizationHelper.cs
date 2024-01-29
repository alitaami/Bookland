using Entities.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Wallet.Common.Resources;
using Wallet.Common.Utilities.Authorization;

public static class AuthorizationHelper
{
    public static IActionResult ValidateAuthorization(string authorizationHeader, int[] roleIds, out string userId)
    {
        userId = null;

        if (string.IsNullOrEmpty(authorizationHeader))
            return Unauthorized(ErrorCodeEnum.AuthorizationHeaderMissing, Resource.AuthorizationHeaderMissing);

        string[] jwtToken = authorizationHeader.Split(" ");

        if (jwtToken.Length != 2 || jwtToken[0] != "Bearer" || jwtToken[1] is null)
            return Unauthorized(ErrorCodeEnum.TokenTypeError, Resource.TokenTypeError);

        // Extract the token without the "Bearer" prefix
        string token = jwtToken[1].Trim(); // Trim leading and trailing whitespaces

        try
        {
            userId = JwtTokenHelper.GetUserIdByClaim(token);
            string roleIdOfUser = JwtTokenHelper.GetUserRoleIdByClaim(token);
            DateTime? expTime = JwtTokenHelper.GetExpirationTime(token);

            // Rest of your validation logic...

            if (!expTime.HasValue)
                return Unauthorized(ErrorCodeEnum.JwtTimeIsNull, Resource.JwtExpTimeClaimMissing);

            if (expTime.Value < DateTime.UtcNow)
                return Unauthorized(ErrorCodeEnum.JwtExpired, Resource.JwtTokenExpired);

            if (roleIdOfUser is null)
                return Unauthorized(ErrorCodeEnum.RoleIdClaimMissing, Resource.RoleIdClaimMissing);

            if (userId is null)
                return Unauthorized(ErrorCodeEnum.UserIdClaimMissing, Resource.UserIdClaimMissing);

            int role_Id = int.Parse(roleIdOfUser);

            // Check if the user's role is in the allowed roleIds
            if (!roleIds.Contains(role_Id))
                return Forbidden(ErrorCodeEnum.PermissionDenied, Resource.RoleDoesNotMatchUser);

            return null; // Indicates successful validation 
        }
        catch (Exception ex)
        {
            // Handle the exception
            // You can log the exception or return a specific error response
            return HandleTokenException(ex);
        }
    }

    private static IActionResult HandleTokenException(Exception ex)
    {
        // Log the exception
        // You can customize this method based on your error handling strategy
        return new ObjectResult(new ServiceResult(null, new ApiResult(HttpStatusCode.Unauthorized, ErrorCodeEnum.UnAuthorized, Resource.LoginAgain, null)))
        {
            StatusCode = (int)HttpStatusCode.Unauthorized
        };
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
