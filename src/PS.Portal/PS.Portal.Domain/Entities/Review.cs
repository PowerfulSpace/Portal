using System.ComponentModel.DataAnnotations;

namespace PS.Portal.Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = "Anonim";
        public string Text { get; set; } = null!;

        [Display(Name = "Movie")]
        public Guid? MovieId { get; set; }
        public Movie? Movie { get; set; }
    }
}
