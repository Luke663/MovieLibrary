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

        /// <summary>
        /// Uses LoadLibraryService to retrieve all database entries for storage in memory.
        /// </summary>
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

        /// <summary>
        /// Finds a specific movie in the library. Returns random movie if passed title
        /// is left Null.
        /// </summary>
        /// <param name="movies">List of all movies held in memeory.</param>
        /// <param name="desiredMovie">Title of desired movie or Null for random movie.</param>
        /// <returns>Movie entity, specific or random.</returns>
        public MovieViewModel GetSingleMovie(IEnumerable<Movie> movies, int? desiredMovieId = null)
        {
            if (movies == null || movies.Count() < 1) // Empty library - return blank
                return new MovieViewModel(new Movie());

            if (desiredMovieId == null)
                return new MovieViewModel(movies.ToList()[new Random().Next(0, movies.Count<Movie>())]);
            else
                return new MovieViewModel(movies.First((a) => a.Id == desiredMovieId));
        }

        /// <summary>
        /// Searches movies held in memory for ones with titles containing the specified parameter. 
        /// </summary>
        /// <returns>Tuple: (list of movies contianing search term, list of genres corresponding to found movies)</returns>
        public (List<MovieViewModel>, List<string>) GetMoviesByTitle(IEnumerable<Movie> movies, string searchTerm)
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

        /// <summary>
        /// Find all movies of a specific genre currently in memory.
        /// </summary>
        public (List<MovieViewModel>, List<string>) GetMovieByGenre(ICollection<Movie> movies, ICollection<Genre> genres, string searchTerm)
        {
            List<MovieViewModel> resultantViewModels = new List<MovieViewModel>();
            HashSet<string> filters = new HashSet<string>();

            List<Movie> results = ((Genre)(genres.First((m) => m.Name == searchTerm))).Movies.ToList();

            foreach (Movie movie in results)
                AddMovieAndCompileFilters(resultantViewModels, filters, movie);

            return (resultantViewModels, filters.ToList());
        }

        /// <summary>
        /// Add a movie and its genres to passed lists.
        /// </summary>
        private void AddMovieAndCompileFilters(List<MovieViewModel> results, HashSet<string> filters, Movie movie)
        {
            results.Add(new MovieViewModel(movie));

            foreach (Genre genre in movie.Genres)
                filters.Add(genre.Name);
        }

        /// <summary>
        /// Inserts recently scraped movie into memory and updates genres accordingly.
        /// </summary>
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

        /// <summary>
        /// Remove a deleted movie and its references from memory.
        /// </summary>
        public void UpdateAfterDeletion(int movieId, List<Movie> movies, List<Genre> genres)
        {
            Movie movieToRemove = movies.Find(m => m.Id == movieId)!;
            string concerningGenres = movieToRemove.GenreString;

            movies.Remove(movieToRemove);

            // Check each genre for references of the deleted movie
            for (int i = 0; i < genres.Count; i++)
            {
                if (!concerningGenres.Contains(genres[i].Name))
                    continue;

                foreach (Movie movie in genres[i].Movies.ToList())
                {
                    if (movie.Id == movieId)
                    {
                        genres[i].Movies.Remove(movie);

                        if (genres[i].Movies.Count() < 1)
                        {
                            DeletionService service = new DeletionService(_contextFactory);
                            service.DeleteGenre(genres[i].Name);

                            genres.RemoveAt(i--);
                        }

                        break;
                    }
                }
            }
        }
    }
}
