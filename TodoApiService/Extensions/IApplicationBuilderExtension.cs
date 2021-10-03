using Microsoft.AspNetCore.Builder;
using TodoApiService.Middlewares;

namespace TodoApiService.Extensions
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseExceptionsHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}