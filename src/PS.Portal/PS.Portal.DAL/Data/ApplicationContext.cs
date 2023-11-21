using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PS.Portal.Domain.Entities;
using PS.Portal.DAL.Data.Configurations;

namespace PS.Portal.DAL.Data
{
    public class ApplicationContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Actor> Actors { get; set; } = null!;
        public DbSet<Producer> Producers { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ActorConfiguration());
            modelBuilder.ApplyConfiguration(new GenreConfiguration());
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
            modelBuilder.ApplyConfiguration(new ProducerConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());

            base.OnModelCreating(modelBuilder);
        }

    }
}
