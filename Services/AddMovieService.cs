using Microsoft.EntityFrameworkCore;
using MovieLibrary.DbContexts;
using MovieLibrary.Models;
using MovieLibrary.Services.Scrape_Services;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Services
{
    class AddMovieService
    {
        private readonly MovieLibraryDbContextFactory _dbContextFactory;
        private readonly LibraryStore _libraryStore;

        public AddMovieService(MovieLibraryDbContextFactory dbContextFactory, LibraryStore libraryStore)
        {
            _dbContextFactory = dbContextFactory;
            _libraryStore = libraryStore;
        }

        /// <summary>
        /// Scrapes a movie via the provided URL and updates the database and library held in memory.
        /// </summary>
        /// <param name="movieURL">IMDb movie page URL for the desired movie.</param>
        /// <returns>Scraped movie entity in ViewModel form for display.</returns>
        public async Task<MovieViewModel> AddMovie(string movieURL)
        {
            // Assemble new movie entity with scraped information
            Scraper scraper = new Scraper();
            Movie movie = scraper.ScrapeMovie(movieURL);

            // Check for duplication
            if (_libraryStore.MovieExists(movie))
                return _libraryStore.GetSingleEntry(movie.Title);

            // Insert and link database entities
            await InitialiseEntities(movie);
            movie = await LinkModelEntities(movie);

            // Update library store in memory
            _libraryStore.UpdateLibraryAfterInsertion(movie, movie.Genres.ToList());

            return new MovieViewModel(movie);
        }

        /// <summary>
        /// Inserts the newly scraped movie and all genres (if not already existing) into the database.
        /// </summary>
        /// <param name="movie">Movie object to be inserted into database.</param>
        /// <returns>Null</returns>
        private async Task InitialiseEntities(Movie movie)
        {
            EntityInsertionService servive = new EntityInsertionService(_dbContextFactory);

            await servive.InsertMovie(movie);

            foreach (string genre in movie.GenreString.Split(','))
                if (genre != "" && !DoesGenreExist(genre.Trim()).Result)
                    await servive.InsertGenre(new Genre() { Name = genre.Trim() });
        }

        /// <summary>
        /// Links Movie and Genre entries in the database facilitating the many-to-many relationship.
        /// </summary>
        /// <param name="movie">Movie being inserted.</param>
        /// <returns>The inserted and linked movie.</returns>
        private async Task<Movie> LinkModelEntities(Movie movie)
        {
            List<Genre> genres = new List<Genre>();

            using (MovieLibraryDbContext context = _dbContextFactory.CreateDbContext())
            {
                movie = await context.Movies.Include(m => m.Genres).FirstAsync(m => m.Title == movie.Title);

                foreach (string genre in movie.GenreString.Split(','))
                    if (genre != "")
                        genres.Add(await context.Genres.Include(g => g.Movies).FirstAsync(g => g.Name == genre.Trim()));

                movie.Genres = genres;

                context.Update(movie);
                await context.SaveChangesAsync();
            }

            return movie;
        }

        /// <summary>
        /// Checks if a genre already exists in the database.
        /// </summary>
        /// <param name="genreName">Genre's name to check for.</param>
        /// <returns>(bool) True if genre exists.</returns>
        private async Task<bool> DoesGenreExist(string genreName)
        {
            Genre? genre = null;

            using (MovieLibraryDbContext context = _dbContextFactory.CreateDbContext())
                genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);

            return genre != null;
        }
    }
}
