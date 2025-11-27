using Microsoft.EntityFrameworkCore;
using Portfolio.Models;
namespace Portfolio
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; } = null!;
    }
}
