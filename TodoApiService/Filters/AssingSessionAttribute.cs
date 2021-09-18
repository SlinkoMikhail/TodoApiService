using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using TodoApiService.Controllers;
using TodoApiService.Extensions;
using TodoApiService.Models;

namespace TodoApiService.Filters
{
    public class AssignSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<TodoApiApplicationContext>();
            if(!(context.Controller is TodoItemsController controller))
            {
                throw new InvalidOperationException($"Контролёр должен наследоваться от {nameof(TodoItemsController)}");
            }
            Guid accountId = context.HttpContext.User.GetAccountId();
            Guid sessionId = context.HttpContext.User.GetSessionId();
            if(accountId == default || sessionId == default)
            {
                context.Result = new BadRequestResult();
                return;
            }
            Session dbSessionFromToken = dbContext.Sessions.FirstOrDefault(s => s.Id == sessionId && s.AccountId == accountId);
            if(dbSessionFromToken == null)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}