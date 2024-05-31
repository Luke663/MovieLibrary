using MovieLibrary.DbContexts;
using MovieLibrary.Models;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Services
{
    // Called by the MoviePageView, used to update the note field of a particular database entry.

    internal class NoteUpdateService
    {
        private readonly MovieLibraryDbContextFactory _dbContextFactory;

        public NoteUpdateService(MovieLibraryDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task UpdateMovie(MovieViewModel movie)
        {
            using (MovieLibraryDbContext context = _dbContextFactory.CreateDbContext())
            {
                Movie movieToAlter = context.Movies.FirstOrDefault(m => m.Id == movie.Id)!;

                movieToAlter.Note = movie.Note;

                context.Update(movieToAlter);
                await context.SaveChangesAsync();
            }
        }
    }
}
