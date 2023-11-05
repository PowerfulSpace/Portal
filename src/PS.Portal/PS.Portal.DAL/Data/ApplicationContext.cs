using Microsoft.EntityFrameworkCore;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; } = null!;
    }
}
