using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Data.Configurations
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder
              .HasMany(x => x.Movies)
              .WithMany(x => x.Genres);
        }
    }
}
