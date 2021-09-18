using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApiService.Models;
using TodoApiService.Extensions;
using TodoApiService.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TodoApiService.Filters;

namespace TodoApiService.Controllers
{
    [ApiController]
    [Route("api/todoitems")]
    [Authorize]
    [AssignSession]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemsRepository _todoItemsRepository;
        private readonly ILogger<TodoItemsController> _logger;
        public TodoItemsController(ITodoItemsRepository todoItemsRepository, ILogger<TodoItemsController> logger)
        {
            _todoItemsRepository = todoItemsRepository;
            _logger = logger;
        }
        [HttpGet]
        public IAsyncEnumerable<TodoItem> GetTodoItems()
        {
            return _todoItemsRepository.GetAll(User.GetAccountId());
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddTodoItem(CreateTodoItem createTodoItem)
        {
            try
            {
                await _todoItemsRepository.AddTodoItem(User.GetAccountId(), createTodoItem);
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
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTodoItems(EditTodoItem todoItem)
        {
            try
            {
                await _todoItemsRepository.UpdateTodoItems(User.GetAccountId(), todoItem);             
                return NoContent();
            }
            catch (System.Exception ex)
            {
                //логануть эксепшен
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{todoItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTodoItem(long todoItemId)
        {
            try
            {
                await _todoItemsRepository.DeleteTodoItem(User.GetAccountId(), todoItemId);
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "");
            }
            return NoContent();
        }
    }
}