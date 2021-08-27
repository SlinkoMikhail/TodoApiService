using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TodoApiService.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            string guid = claimsPrincipal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");//??
            return Guid.TryParse(guid, out var id) ? id : Guid.Empty;
        }
    }
}