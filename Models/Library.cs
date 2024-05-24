using MovieLibrary.DbContexts;
using MovieLibrary.Services;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Models
{
    class Library
    {
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public Library(MovieLibraryDbContextFactory context)
        {
            _contextFactory = context;
        }

        public (List<Movie>, List<Genre>) LoadLibrary()
        {
            LoadLibraryService service = new LoadLibraryService(_contextFactory);

            try
            {
                return (service.GetAllMovies().Result, service.GetAllGenres().Result);
            }
            catch (System.AggregateException)
            {
                return (new List<Movie>(), new List<Genre>());
            }
        }

        public MovieViewModel GetSingleMovie(IEnumerable<Movie> movies, string? desiredMovie = null)
        {
            if (movies == null || movies.Count() < 1)
                return new MovieViewModel(new Movie());

            if (desiredMovie == null)
                return new MovieViewModel(movies.ToList()[new Random().Next(0, movies.Count<Movie>())]);
            else
                return new MovieViewModel(movies.First((a) => a.Title == desiredMovie));
        }

        public (List<MovieViewModel>, List<string>) GetMovieByTitle(IEnumerable<Movie> movies, string searchTerm)
        {
            List<MovieViewModel> results = new List<MovieViewModel>();
            HashSet<string> filters = new HashSet<string>();

            foreach (Movie movie in movies)
            {
                if (movie.Title.Contains(searchTerm))
                {
                    results.Add(new MovieViewModel(movie));

                    foreach (Genre genre in movie.Genres)
                        filters.Add(genre.Name);
                }
            }

            return (results, filters.ToList<string>());
        }

        public (List<MovieViewModel>, List<string>) GetMovieByGenre(ICollection<Movie> movies, ICollection<Genre> genres, string searchTerm)
        {
            List<MovieViewModel> resultantViewModels = new List<MovieViewModel>();
            HashSet<string> filters = new HashSet<string>();

            List<Movie> results = ((Genre)(genres.First((m) => m.Name == searchTerm))).Movies.ToList();

            foreach (Movie movie in results)
                AddMovieAndFilters(resultantViewModels, filters, movie);

            return (resultantViewModels, filters.ToList());
        }

        private void AddMovieAndFilters(List<MovieViewModel> results, HashSet<string> filters, Movie movie)
        {
            results.Add(new MovieViewModel(movie));

            foreach (Genre genre in movie.Genres)
                filters.Add(genre.Name);
        }

        public void UpdateAfterInsertion(Movie insertedMovie, List<Genre> newGenres, List<Movie> movies, List<Genre> genres)
        {
            movies.Add(insertedMovie);

            foreach (Genre genre in newGenres)
            {
                if (genres.Exists(g => g.Name == genre.Name))
                    genres[genres.FindIndex(g => g.Name == genre.Name)] = genre; // Replace with updated version
                else
                    genres.Add(genre);
            }
        }

        public void UpdateAfterDeletion(string movieTitle, List<Movie> movies, List<Genre> genres)
        {
            Movie movieToRemove = movies.Find(m => m.Title == movieTitle)!;

            movies.Remove(movieToRemove);

            for (int i = 0; i < genres.Count; i++)
                foreach (Movie movie in genres[i].Movies.ToList())
                    if (movie.Title == movieTitle)
                    {
                        genres[i].Movies.Remove(movie);

                        if (genres[i].Movies.Count() < 1)
                        {
                            GenreDeleteService service = new GenreDeleteService(_contextFactory);
                            service.DeleteGenre(genres[i].Name);

                            genres.RemoveAt(i--);
                        }

                        break;
                    }
        }
    }
}
