using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApiService.Models;
using TodoApiService.Extensions;
using System.Linq;

namespace TodoApiService.Controllers
{
    [ApiController]
    [Route("api/todoitems")]
    [Authorize]
    public class TodoItemsController : ControllerBase
    {
        private readonly IAccountManager _accountManager;
        public TodoItemsController(IAccountManager accountManager)
        {
            _accountManager = accountManager;   
        }
        //GET all items
        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            throw new NotImplementedException();
        }
        //POST(TodoItem) add(TodoItems)
        [HttpPost]
        public async Task<IActionResult> AddTodoItem(TodoItem todoItem)
        {
            throw new NotImplementedException();
        }
        //UPDATE(arr TodoIems)
        [HttpPut]
        public async Task<IActionResult> UpdateTodoItems(TodoItem[] todoItems)
        {
            throw new NotImplementedException();
        }
        //DELETE(id) remove(id)
        [HttpDelete("{id}")]
        public async Task DeleteTodoItem(Guid id)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        private async Task<Account> GetAuthenticatedUser()
        {
            return await _accountManager.GetAccountByIdAsync(User.GetUserId());
        }
    }
}