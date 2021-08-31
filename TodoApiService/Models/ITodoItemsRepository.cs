using System;
using System.Collections.Generic;

namespace TodoApiService.Models
{
    public interface ITodoItemsRepository
    {
        IEnumerable<TodoItem> GetAll(Guid userId);
        void AddTodoItem(Guid userId, TodoItem item);
        void DeleteTodoItem(Guid userId, long todoItemId);
        void UpdateTodoItems(Guid userId, TodoItem newItem);
    }
}