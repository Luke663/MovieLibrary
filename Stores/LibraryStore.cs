using MovieLibrary.Models;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Stores
{
    internal class LibraryStore
    {
        private readonly Library _library;

        private List<Movie> _movies = new List<Movie>();
        private List<Genre> _genres = new List<Genre>();

        public IEnumerable<Movie> Movies => _movies;
        public IEnumerable<Genre> Genres => _genres;

        public LibraryStore(Library library)
        {
            _library = library;

            (_movies, _genres) = _library.LoadLibrary();
        }

        public MovieViewModel GetSingleEntry(string? desiredEntry = null)
        {
            return _library.GetSingleMovie(_movies, desiredEntry);
        }

        public (List<MovieViewModel>, List<string>) GetMovieByTitle(string searchTerm)
        {
            return _library.GetMovieByTitle(_movies, searchTerm);
        }

        public (List<MovieViewModel>, List<string>) GetMoviesByGenre(string searchTerm)
        {
            return _library.GetMovieByGenre(_movies, _genres, searchTerm);
        }

        public void UpdateStoreAfterDeletion(string movieTitle)
        {
            _library.UpdateAfterDeletion(movieTitle, _movies, _genres);
        }

        public void UpdateLibraryAfterInsertion(Movie insertedMovie, List<Genre> newGenres)
        {
            _library.UpdateAfterInsertion(insertedMovie, newGenres, _movies, _genres);
        }

        public void UpdateAfterNoteAlteration(MovieViewModel movie)
        {
            _movies[_movies.FindIndex(m => m.Id == movie.Id)].Note = movie.Note;
        }

        public bool MovieExists(Movie movie)
        {
            return _movies.Find(m => m.Title == movie.Title) != null;
        }
    }
}
