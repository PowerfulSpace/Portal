using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder
               .HasOne(x => x.Movie)
               .WithMany(x => x.Reviews)
               .HasForeignKey(x => x.MovieId);

            builder.Property(x => x.Login).HasMaxLength(25);
            builder.Property(x => x.Text).IsRequired();
        }
    }
}
