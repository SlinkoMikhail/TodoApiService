using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApiService.Models.DTO;

namespace TodoApiService.Models
{
    public interface ITodoItemsRepository
    {
        IAsyncEnumerable<TodoItem> GetAll(Guid userId);
        Task AddTodoItem(Guid userId, CreateTodoItem item);
        Task DeleteTodoItem(Guid userId, long todoItemId);
        Task UpdateTodoItems(Guid userId, EditTodoItem newItem);
    }
}