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
        public async Task<IActionResult> GetTodoItems()
        {
            
            return Ok(_todoItemsRepository.GetAll(User.GetUserId()));
        }
        //POST(TodoItem) add(TodoItems)
        [HttpPost]
        public async Task<IActionResult> AddTodoItem(TodoItem todoItem)
        {
            _todoItemsRepository.AddTodoItem(User.GetUserId(), todoItem);
            return Ok();
        }
        //UPDATE(arr TodoIems)
        [HttpPut]
        public async Task<IActionResult> UpdateTodoItems(TodoItem todoItem)
        {
            _todoItemsRepository.UpdateTodoItems(User.GetUserId(), todoItem);
            return Ok();
        }
        //DELETE(id) remove(id)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            _todoItemsRepository.DeleteTodoItem(User.GetUserId(), id);
            return Ok();
        }
    }
}