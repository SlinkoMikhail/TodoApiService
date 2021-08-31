using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoApiService.Models
{
    public class EFTodoItemsRepository : ITodoItemsRepository
    {
        private readonly TodoApiApplicationContext _appDbContext;
        public EFTodoItemsRepository(TodoApiApplicationContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> AddTodoItem(Guid userId, TodoItem item)
        {
            if(item.AccountId == userId)
            {
                await _appDbContext.TodoItems.AddAsync(item);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task DeleteTodoItem(Guid userId, long todoItemId)
        {
            TodoItem item = await _appDbContext.TodoItems.FindAsync(todoItemId);
            if(item?.AccountId == userId)
            {
                _appDbContext.TodoItems.Remove(item);
                await _appDbContext.SaveChangesAsync();
            }
        }

        public IAsyncEnumerable<TodoItem> GetAll(Guid id)
        {
            return _appDbContext.TodoItems.Where(tdi => tdi.AccountId == id).AsAsyncEnumerable();
        }

        public async Task UpdateTodoItems(Guid userId, TodoItem newItem)
        {
            TodoItem storedItem = await _appDbContext.TodoItems.FirstOrDefaultAsync(tdi => tdi.Id == newItem.Id && tdi.AccountId == userId);
            if(storedItem != null)
            {
                storedItem.IsComplete = newItem.IsComplete;
                storedItem.IsImportant = newItem.IsImportant;
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}