using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TodoApiService.Models;

namespace TodoApiService.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static Guid GetAccountId(this ClaimsPrincipal claims) => GetValueFromJWT(claims, ClaimNames.UniqueId);
        public static Guid GetSessionId(this ClaimsPrincipal claims) => GetValueFromJWT(claims, ClaimNames.SessionId);
        public static Guid GetRefresh(this ClaimsPrincipal claims) => GetValueFromJWT(claims, JwtRegisteredClaimNames.Jti);
        private static Guid GetValueFromJWT(ClaimsPrincipal claims, string key)
        {
            string guid = claims.FindFirstValue(key);
            return Guid.TryParse(guid, out var id) ? id : default;
        }
    }
}