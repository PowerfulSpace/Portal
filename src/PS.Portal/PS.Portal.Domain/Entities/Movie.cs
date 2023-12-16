using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PS.Portal.Domain.Entities
{
    public class Movie
    {
        public Guid Id { get; set; }

        [Display(Name = "Название фильма")]
        public string Name { get; set; } = null!;

        [Display(Name = "Описание")]
        public string Description { get; set; } = null!;

        [Range(0, 10)]
        [Display(Name = "Рейтинг")]
        public double Rating { get; set; }

        [Display(Name = "Год выпуска")]
        public DateTime ReleaseYear { get; set; }

        [Display(Name = "Продолжительность фильма")]
        public int FilmDuration { get; set; }


        [Display(Name = "Приемлемый возраст")]
        public int AcceptableAge { get; set; }
        [Display(Name = "Просмотрен")]
        public bool IsReaded { get; set; }
        [Display(Name = "Часть")]
        public int? PartFilm { get; set; }


        public string PhotoUrl { get; set; } = "noimage.png";

        [Display(Name = "MoviePhoto")]
        [NotMapped]
        public IFormFile? MoviePhoto { get; set; }

        [NotMapped]
        public string BreifPhotoName { get; set; } = null!;


        [Display(Name = "Страна")]
        public Guid? CountryId { get; set; }
        public Country? Country { get; set; }


        [Display(Name = "режиссёр")]
        public Guid? ProducerId { get; set; }
        public Producer? CurrentProducer { get; set; }

        [Display(Name = "Актёры")]
        public List<Actor> Actors { get; set; } = new List<Actor>();

        [Display(Name = "Жанры")]
        public List<Genre> Genres { get; set; } = new List<Genre>();

        [Display(Name = "Рецензии")]
        public List<Review> Reviews { get; set; } = new List<Review>();

    }
}
