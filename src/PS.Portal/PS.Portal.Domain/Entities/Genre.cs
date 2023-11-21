namespace PS.Portal.Domain.Entities
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}
