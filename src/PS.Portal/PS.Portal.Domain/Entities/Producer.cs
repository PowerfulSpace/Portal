using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PS.Portal.Domain.Entities
{
    public class Producer
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

        [Display(Name = "ProducerPhoto")]
        [NotMapped]
        public IFormFile ProducerPhoto { get; set; } = null!;

        [InverseProperty("CurrentProducer")]
        public virtual List<Movie>? Movies { get; set; }
    }
}
