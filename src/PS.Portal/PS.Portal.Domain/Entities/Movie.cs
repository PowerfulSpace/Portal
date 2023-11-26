using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PS.Portal.Domain.Entities
{
    public class Movie
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Range(0, 10)]
        [Display(Name = "Rating")]
        public double Rating { get; set; }

        [Display(Name = "YearShown")]
        public DateTime YearShown { get; set; }

        [Display(Name = "FilmDuration")]
        public int FilmDuration { get; set; }

        public int AcceptableAge { get; set; }
        public bool IsReaded { get; set; }
        public int? PartFilm { get; set; }


        public string PhotoUrl { get; set; } = "noimage.png";

        [Display(Name = "MoviePhoto")]
        [NotMapped]
        public IFormFile MoviePhoto { get; set; } = null!;

        [NotMapped]
        public string BreifPhotoName { get; set; } = null!;


        [Display(Name = "Country")]
        public Guid? CountryId { get; set; }
        public Country? Country { get; set; }


        public Guid? ProducerId { get; set; }
        [Display(Name = "Producer")]
        public Producer? CurrentProducer { get; set; }

        public List<Actor> Actors { get; set; } = new List<Actor>();

        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Review> Reviews { get; set; } = new List<Review>();

    }
}
