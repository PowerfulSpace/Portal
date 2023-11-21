namespace PS.Portal.Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = "Anonim";
        public string Text { get; set; } = null!;
        public Guid? MovieId { get; set; }
        public Movie? Movie { get; set; }
    }
}
