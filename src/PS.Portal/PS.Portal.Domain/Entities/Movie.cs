﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PS.Portal.Domain.Entities
{
    public class Movie
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(25)]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(75)]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0, 10)]
        [Display(Name = "Rating")]
        public double Rating { get; set; }

        [Required]
        [Display(Name = "YearShown")]
        public DateTime YearShown { get; set; }

       
        [Required]
        [Display(Name = "FilmDuration")]
        public int FilmDuration { get; set; }

        [Required]
        [StringLength(25)]
        [Display(Name = "Country")]
        public string Country { get; set; } = null!;

        [Required]
        public int AcceptableAge { get; set; }


        public string PhotoUrl { get; set; } = "noimage.png";

        [Display(Name = "MoviePhoto")]
        [NotMapped]
        public IFormFile MoviePhoto { get; set; } = null!;


        [Display(Name = "Producer")]
        public Guid? ProducerId { get; set; }
        public Producer? CurrentProducer { get; set; }

        public List<Actor> Actors { get; set; } = new List<Actor>();

        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Review> Reviews { get; set; } = new List<Review>();

    }
}
