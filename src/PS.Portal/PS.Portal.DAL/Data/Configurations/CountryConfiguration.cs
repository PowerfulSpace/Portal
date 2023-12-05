using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder
               .HasMany(x => x.Movies)
               .WithOne(x => x.Country)
               .HasForeignKey(x => x.CountryId);

            builder
               .HasMany(x => x.Actors)
               .WithOne(x => x.Country)
               .HasForeignKey(x => x.CountryId);

            builder
               .HasMany(x => x.Producers)
               .WithOne(x => x.Country)
               .HasForeignKey(x => x.CountryId);


            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        }
    }
}