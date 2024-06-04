using Microsoft.EntityFrameworkCore;
using MovieLibrary.DbContexts;
using MovieLibrary.Models;

namespace MovieLibrary.Services
{
    // Uses an entity's title or name to find and remove that entity (Genre or Movie) from the database.

    internal class DeletionService
    {
        private MovieLibraryDbContextFactory _contextFactory;

        public DeletionService(MovieLibraryDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async void DeleteMovie(int? movieId)
        {
            if (movieId == null)
                return;

            using (MovieLibraryDbContext context = _contextFactory.CreateDbContext())
            {
                Movie entry = await context.Movies.Include(m => m.Genres).FirstAsync(m => m.Id == movieId);
                context.Movies.Remove(entry);

                await context.SaveChangesAsync();
            }
        }

        public async void DeleteGenre(string genreName)
        {
            if (genreName == "")
                return;

            using (MovieLibraryDbContext context = _contextFactory.CreateDbContext())
            {
                Genre genre = context.Genres.FirstOrDefault(g => g.Name == genreName)!;
                context.Genres.Remove(genre);

                await context.SaveChangesAsync();
            }
        }
    }
}
