using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Data.Configurations
{
    public class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            builder
                .HasMany(x => x.Movies)
                .WithMany(x => x.Actors);
        }
    }
}
