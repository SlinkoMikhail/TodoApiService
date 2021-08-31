using System;
using System.Collections.Generic;
namespace TodoApiService.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string HashPassword { get; set; }
        public Roles Role { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
        public RefreshToken Token { get; set; }
    }
}