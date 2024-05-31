using Microsoft.EntityFrameworkCore;
using MovieLibrary.DbContexts;
using MovieLibrary.Models;

namespace MovieLibrary.Services
{
    // Service for loading library; returns either all Movies or all Genres

    internal class LoadLibraryService
    {
        private MovieLibraryDbContextFactory _contextFactory;

        public LoadLibraryService(MovieLibraryDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Movie>> GetAllMovies()
        {
            using (MovieLibraryDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Movies.Include(m => m.Genres).ToListAsync();
            }
        }

        public async Task<List<Genre>> GetAllGenres()
        {
            using (MovieLibraryDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Genres.Include(m => m.Movies).ToListAsync();
            }
        }
    }
}
