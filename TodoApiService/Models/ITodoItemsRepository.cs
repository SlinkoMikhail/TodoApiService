using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApiService.Models
{
    public interface ITodoItemsRepository
    {
        IAsyncEnumerable<TodoItem> GetAll(Guid userId);
        Task<bool> AddTodoItem(Guid userId, TodoItem item);
        Task DeleteTodoItem(Guid userId, long todoItemId);
        Task UpdateTodoItems(Guid userId, TodoItem newItem);
    }
}