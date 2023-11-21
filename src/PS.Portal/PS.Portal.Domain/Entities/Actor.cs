using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PS.Portal.Domain.Entities
{
    public class Actor
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;   

        public DateTime BirthDate { get; set; }

        public string PhotoUrl { get; set; } = "noimage.png";

        [Display(Name = "ActorPhoto")]
        [NotMapped]
        public IFormFile ActorPhoto { get; set; } = null!;


        [Display(Name = "Country")]
        public Guid? CountryId { get; set; }
        public Country? Country { get; set; }


        public List<Movie> Movies { get; set; } = new List<Movie>();

    }
}
