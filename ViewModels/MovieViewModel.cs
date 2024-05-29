using MovieLibrary.Models;
using System.IO;

namespace MovieLibrary.ViewModels
{
    // Used as the display type for movie objects

    class MovieViewModel
    {
        private readonly Movie _movie;

        public int Id => _movie.Id;
        public string Title => _movie.Title;
        public string Description => _movie.Description;
        public string GenreString => _movie.GenreString;
        public string Path => Directory.GetCurrentDirectory() + @"\" + _movie.Path;
        public string Date => _movie.Date;
        public string AgeRating => "Rated: " + _movie.AgeRating;
        public string Duration => _movie.Duration;
        public string Score => _movie.Score;

        public string Note
        {
            get => _movie.Note;
            set => _movie.Note = value;
        }

        public MovieViewModel(Movie movie)
        {
            _movie = movie;
        }
    }
}
