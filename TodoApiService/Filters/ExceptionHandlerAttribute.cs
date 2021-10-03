using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace TodoApiService.Filters
{
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if(context.ExceptionHandled) return;
            switch (context.Exception)
            {
                case SecurityTokenException ste:
                {
                    Log.Warning(ste.ToString());
                    context.Result = new UnauthorizedObjectResult(ste.Message);
                    break;
                }
                default:
                    Log.Fatal(context.Exception.ToString());
                    context.Result = new BadRequestObjectResult(context.Exception.Message);
                    break;
            }
        }
    }
}