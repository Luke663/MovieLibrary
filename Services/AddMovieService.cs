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

        public async Task<MovieViewModel> AddMovie(string movieURL)
        {
            Scraper scraper = new Scraper();
            Movie movie = scraper.ScrapeMovie(movieURL);

            if (_libraryStore.MovieExists(movie))
                return _libraryStore.GetSingleEntry(movie.Title);

            await InitialiseEntities(movie);
            movie = await LinkModelEntities(movie);

            _libraryStore.UpdateLibraryAfterInsertion(movie, movie.Genres.ToList());
            return new MovieViewModel(movie);
        }

        private async Task InitialiseEntities(Movie movie)
        {
            EntityInsertionService servive = new EntityInsertionService(_dbContextFactory);

            await servive.InsertMovie(movie);

            foreach (string genre in movie.GenreString.Split(','))
                if (genre != "" && !DoesGenreExist(genre.Trim()).Result)
                    await servive.InsertGenre(new Genre() { Name = genre.Trim() });
        }

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

        private async Task<bool> DoesGenreExist(string genreName)
        {
            Genre? genre = null;

            using (MovieLibraryDbContext context = _dbContextFactory.CreateDbContext())
                genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);

            return genre != null;
        }


    }
}
