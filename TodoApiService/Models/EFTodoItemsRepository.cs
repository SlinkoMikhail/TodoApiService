using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApiService.Models
{
    public class EFTodoItemsRepository : ITodoItemsRepository
    {
        private readonly TodoApiApplicationContext _appDbContext;
        public EFTodoItemsRepository(TodoApiApplicationContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void AddTodoItem(Guid userId, TodoItem item)
        {
            if(item.AccountId == userId)
            {
                _appDbContext.TodoItems.Add(item);
                _appDbContext.SaveChanges();
            }
        }

        public void DeleteTodoItem(Guid userId, long todoItemId)
        {
            TodoItem item = _appDbContext.TodoItems.Find(todoItemId);
            if(item?.AccountId == userId)
            {
                _appDbContext.TodoItems.Remove(item);
                _appDbContext.SaveChanges();
            }
        }

        public IEnumerable<TodoItem> GetAll(Guid id)
        {
            return _appDbContext.TodoItems.Where(tdi => tdi.AccountId == id).AsEnumerable();
        }

        public void UpdateTodoItems(Guid userId, TodoItem newItem)
        {
            TodoItem storedItem = _appDbContext.TodoItems.FirstOrDefault(tdi => tdi.Id == newItem.Id && tdi.AccountId == userId);
            if(storedItem != null)
            {
                storedItem.IsComplete = newItem.IsComplete;
                storedItem.IsImportant = newItem.IsImportant;
                _appDbContext.SaveChanges();
            }
        }
    }
}