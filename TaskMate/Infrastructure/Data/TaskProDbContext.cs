using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskMate.Core.Models;

namespace TaskMate.Infrastructure.Data
{
    public class TaskProDbContext : IdentityDbContext<User>
    {

        public TaskProDbContext(DbContextOptions<TaskProDbContext> options)  : base(options) { }

        public DbSet<User> Users { get; set; }

        //For RefreshTokens
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}
