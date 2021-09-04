using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApiService.Models;
using TodoApiService.Extensions;
using TodoApiService.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TodoApiService.Controllers
{
    [ApiController]
    [Route("api/todoitems")]
    [Authorize]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemsRepository _todoItemsRepository;
        private readonly ILogger<TodoItemsController> _logger;
        public TodoItemsController(ITodoItemsRepository todoItemsRepository, ILogger<TodoItemsController> logger)
        {
            _todoItemsRepository = todoItemsRepository;
            _logger = logger;
        }
        //GET all items
        [HttpGet]
        public IAsyncEnumerable<TodoItem> GetTodoItems()
        {
            return _todoItemsRepository.GetAll(User.GetUserId());
        }
        //POST(TodoItem) add(TodoItems)
        [HttpPost]
        public async Task<IActionResult> AddTodoItem(CreateTodoItem createTodoItem)
        {
            try
            {
                await _todoItemsRepository.AddTodoItem(User.GetUserId(), createTodoItem);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (System.Exception ex)
            {
                //логануть и отправить ответ
                string message = "usefull message";
                _logger.LogWarning(ex, message);
                return BadRequest(ex.Message);
            }
        }
        //UPDATE(arr TodoIems)
        [HttpPut]
        public async Task<IActionResult> UpdateTodoItems(EditTodoItem todoItem)
        {
            try
            {
                await _todoItemsRepository.UpdateTodoItems(User.GetUserId(), todoItem);             
                return NoContent();
            }
            catch (System.Exception ex)
            {
                //логануть эксепшен
                return BadRequest(ex.Message);
            }
        }
        //DELETE(id) remove(id)
        [HttpDelete("{todoItemId}")]
        public async Task<IActionResult> DeleteTodoItem(long todoItemId)
        {
            try
            {
                await _todoItemsRepository.DeleteTodoItem(User.GetUserId(), todoItemId);
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "");
            }
            return NoContent();
        }
    }
}