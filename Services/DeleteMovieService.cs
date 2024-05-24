using Microsoft.EntityFrameworkCore;
using MovieLibrary.DbContexts;
using MovieLibrary.Models;

namespace MovieLibrary.Services
{
    internal class DeleteMovieService
    {
        private MovieLibraryDbContextFactory _contextFactory;

        public DeleteMovieService(MovieLibraryDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async void DeleteMovie(string movieTitle)
        {
            using (MovieLibraryDbContext context = _contextFactory.CreateDbContext())
            {
                Movie entry = await context.Movies.Include(m => m.Genres).FirstAsync(m => m.Title == movieTitle);
                context.Movies.Remove(entry);

                await context.SaveChangesAsync();
            }
        }
    }
}
