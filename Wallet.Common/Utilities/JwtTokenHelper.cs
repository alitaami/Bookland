using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Common.Utilities
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
    }
}
