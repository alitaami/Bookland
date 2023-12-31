﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Wallet.Common.Utilities.Authorization
{
    public class JwtTokenHelper
    {
        public static ClaimsPrincipal GetClaimsPrincipalFromToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

            if (jsonToken != null)
            {
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jsonToken.Claims, "jwt"));
                return claimsPrincipal;
            }

            return null;
        }

        public static string GetClaimValue(string jwtToken, string claimType)
        {
            var claimsPrincipal = GetClaimsPrincipalFromToken(jwtToken);

            if (claimsPrincipal != null)
            {
                var claim = claimsPrincipal.FindFirst(claimType);
                return claim?.Value;
            }

            return null;
        }

        public static string GetUserIdByClaim(string jwtToken)
        {
            var userId = GetClaimValue(jwtToken, "user_id");
            return userId;
        }
        public static string GetUserRoleIdByClaim(string jwtToken)
        {
            var userId = GetClaimValue(jwtToken, "role_id");
            return userId;
        }

        public static DateTime? GetExpirationTime(string jwtToken)
        {
            var expValue = GetClaimValue(jwtToken, "exp");

            if (expValue != null && long.TryParse(expValue, out long expUnixTime))
            {
                return DateTimeOffset.FromUnixTimeSeconds(expUnixTime).UtcDateTime;
            }

            return null;
        }
    }
}
