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
        private readonly ITodoItemsRepository _todoItemsRepository;
        public TodoItemsController(ITodoItemsRepository todoItemsRepository)
        {
            _todoItemsRepository = todoItemsRepository;
        }
        //GET all items
        [HttpGet]
        public IAsyncEnumerable<TodoItem> GetTodoItems()
        {
            return _todoItemsRepository.GetAll(User.GetUserId());
        }
        //POST(TodoItem) add(TodoItems)
        [HttpPost]
        public async Task<IActionResult> AddTodoItem(TodoItem todoItem)
        {
            bool success = await _todoItemsRepository.AddTodoItem(User.GetUserId(), todoItem);
            if(success)
                return Ok();
            return BadRequest();
        }
        //UPDATE(arr TodoIems)
        [HttpPut]
        public async Task<IActionResult> UpdateTodoItems(TodoItem todoItem)
        {
            await _todoItemsRepository.UpdateTodoItems(User.GetUserId(), todoItem);
            return NoContent();
        }
        //DELETE(id) remove(id)
        [HttpDelete("{todoItemId}")]
        public async Task<IActionResult> DeleteTodoItem(long todoItemId)
        {
            await _todoItemsRepository.DeleteTodoItem(User.GetUserId(), todoItemId);
            return NoContent();
        }
    }
}