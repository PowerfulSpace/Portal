using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PS.Portal.Domain.Entities
{
    public class Actor
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(25)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(25)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(25)]
        public string Country { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string PhotoUrl { get; set; } = "noimage.png";

        [Display(Name = "ActorPhoto")]
        [NotMapped]
        public IFormFile ActorPhoto { get; set; } = null!;

        public virtual List<Movie>? Movies { get; set; }

    }
}
