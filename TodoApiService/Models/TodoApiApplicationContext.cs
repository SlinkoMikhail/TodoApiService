using Microsoft.EntityFrameworkCore;

namespace TodoApiService.Models
{
    public class TodoApiApplicationContext : DbContext
    {
        public TodoApiApplicationContext(DbContextOptions<TodoApiApplicationContext> options) : base(options){}
        public DbSet<Account> Accounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email).IsUnique();
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Phone).IsUnique();
        }
    }
}