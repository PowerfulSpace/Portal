using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Data.Configurations
{
    public class ProducerConfiguration : IEntityTypeConfiguration<Producer>
    {
        public void Configure(EntityTypeBuilder<Producer> builder)
        {
            builder
              .HasMany(x => x.Movies)
              .WithOne(x => x.CurrentProducer)
              .HasForeignKey(x => x.ProducerId);

            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(25);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(25);
            builder.Property(x => x.BirthDate).IsRequired();
            builder.Property(x => x.PhotoUrl).IsRequired();
        }
    }
}
