using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace TodoApiService.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            switch (exception)
            {
                case SecurityTokenException securityException:
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    Log.Warning(securityException.ToString());
                    await context.Response.WriteAsync(exception.Message);
                    break;
                }
                case ArgumentNullException errorException:
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    Log.Error(errorException.ToString());
                    await context.Response.WriteAsync(exception.Message);
                    break;
                }
                
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    Log.Fatal(exception.ToString());
                    await context.Response.WriteAsync("Internal server error.");
                    break;
            }
        }
    }
}