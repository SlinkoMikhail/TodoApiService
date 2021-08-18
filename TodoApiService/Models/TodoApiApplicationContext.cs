using Microsoft.EntityFrameworkCore;

namespace TodoApiService.Models
{
    public class TodoApiApplicationContext : DbContext
    {
        public TodoApiApplicationContext(DbContextOptions<TodoApiApplicationContext> options) : base(options){}
        public DbSet<Account> Accounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}