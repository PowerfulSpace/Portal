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
        }
    }
}
