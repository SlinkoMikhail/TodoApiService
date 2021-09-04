using System;
using System.Security.Claims;
using TodoApiService.Models;

namespace TodoApiService.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            string guid = claimsPrincipal.FindFirstValue(ClaimNames.UniqueId);
            return Guid.TryParse(guid, out var id) ? id : Guid.Empty;
        }
    }
}