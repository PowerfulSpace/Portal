namespace PS.Portal.Domain.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<Producer> Producers { get; set; } = new List<Producer>();
    }
}
