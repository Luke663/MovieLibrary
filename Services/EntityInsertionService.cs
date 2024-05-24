using Microsoft.EntityFrameworkCore.ChangeTracking;
using MovieLibrary.DbContexts;
using MovieLibrary.Models;

namespace MovieLibrary.Services
{
    internal class EntityInsertionService
    {
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public EntityInsertionService(MovieLibraryDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Movie> InsertMovie(Movie movie)
        {
            using (MovieLibraryDbContext context = _contextFactory.CreateDbContext())
            {
                EntityEntry<Movie> createdResult = await context.Movies.AddAsync(movie);
                await context.SaveChangesAsync();

                return createdResult.Entity;
            }
        }

        public async Task<Genre> InsertGenre(Genre genre)
        {
            using (MovieLibraryDbContext context = _contextFactory.CreateDbContext())
            {
                EntityEntry<Genre> createdResult = await context.Genres.AddAsync(genre);
                await context.SaveChangesAsync();

                return createdResult.Entity;
            }
        }
    }
}
