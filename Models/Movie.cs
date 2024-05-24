namespace MovieLibrary.Models
{
    class Movie : ModelObject
    {
        public string Title { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string GenreString { get; set; } = String.Empty;
        public string Path { get; set; } = String.Empty;
        public string Date { get; set; } = String.Empty;
        public string AgeRating { get; set; } = String.Empty;
        public string Score { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public string Duration { get; set; } = String.Empty;

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
