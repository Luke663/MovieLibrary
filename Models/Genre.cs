namespace MovieLibrary.Models
{
    class Genre : ModelObject
    {
        public string Name { get; set; } = String.Empty;

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
