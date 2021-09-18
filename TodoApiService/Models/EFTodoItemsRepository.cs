using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApiService.Models.DTO;

namespace TodoApiService.Models
{
    public class EFTodoItemsRepository : ITodoItemsRepository
    {
        private readonly TodoApiApplicationContext _appDbContext;
        public EFTodoItemsRepository(TodoApiApplicationContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddTodoItem(Guid userId, CreateTodoItem item)
        {
            TodoItem todoItem = new TodoItem
            {
                Id = 0,
                AccountId = userId,
                Description = item.Description,
                IsComplete = item.IsComplete,
                IsImportant = item.IsImportant,
                Title = item.Title
            };
            await _appDbContext.TodoItems.AddAsync(todoItem);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteTodoItem(Guid userId, long todoItemId)
        {
            TodoItem item = await _appDbContext.TodoItems.FindAsync(todoItemId);
            if(item?.AccountId == userId)
            {
                _appDbContext.TodoItems.Remove(item);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                //exception о том, что не соответсует idшки
                throw new Exception("The Id of the authenticated user does not match the Id of the user who performs the action.");
            }
        }

        public IAsyncEnumerable<TodoItem> GetAll(Guid id)
        {
            return _appDbContext.TodoItems.Where(tdi => tdi.AccountId == id).AsAsyncEnumerable();
        }

        public async Task UpdateTodoItems(Guid userId, EditTodoItem newItem)
        {
            TodoItem storedItem = await _appDbContext.TodoItems.FirstOrDefaultAsync(tdi => 
                tdi.Id == newItem.Id && tdi.AccountId == userId);
            if(storedItem != null)
            {
                storedItem.IsComplete = newItem.IsComplete;
                storedItem.IsImportant = newItem.IsImportant;
                storedItem.Description = newItem.Description;
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                //exception о том, что не найдено такой задачи для апдейта
                throw new Exception("The Id of the authenticated user does not match the Id of the user who performs the action.");
            }
        }
    }
}