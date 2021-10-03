using System;
using System.Security.Claims;
using TodoApiService.Models;

namespace TodoApiService.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static Guid GetAccountId(this ClaimsPrincipal claims) => GetValueFromJWT(claims, ClaimNames.AccountId);
        public static Guid GetSessionId(this ClaimsPrincipal claims) => GetValueFromJWT(claims, ClaimNames.SessionId);
        public static Guid GetRefresh(this ClaimsPrincipal claims) => GetValueFromJWT(claims, ClaimNames.RefreshBase);
        private static Guid GetValueFromJWT(ClaimsPrincipal claims, string key)
        {
            string guid = claims.FindFirstValue(key);
            return Guid.TryParse(guid, out var id) ? id : default;
        }
    }
}