using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Data.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder
                 .HasOne(x => x.CurrentProducer)
                 .WithMany(x => x.Movies)
                 .HasForeignKey(x => x.ProducerId);

            builder
                .HasMany(x => x.Reviews)
                .WithOne(x => x.Movie)
                .HasForeignKey(x => x.MovieId);

            builder
                .HasMany(x => x.Actors)
                .WithMany(x => x.Movies);

            builder
               .HasMany(x => x.Genres)
               .WithMany(x => x.Movies);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(25);
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Rating).IsRequired();
            builder.Property(x => x.ReleaseYear).IsRequired();
            builder.Property(x => x.FilmDuration).IsRequired();
            builder.Property(x => x.AcceptableAge).IsRequired();
            builder.Property(x => x.PhotoUrl).IsRequired();
            builder.Property(x => x.IsReaded).IsRequired();
            builder.Property(x => x.PartFilm);
        }
    }
}
