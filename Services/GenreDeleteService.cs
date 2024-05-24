using MovieLibrary.DbContexts;
using MovieLibrary.Models;

namespace MovieLibrary.Services
{
    class GenreDeleteService
    {
        private MovieLibraryDbContextFactory _contextFactory;

        public GenreDeleteService(MovieLibraryDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async void DeleteGenre(string genreName)
        {
            using (MovieLibraryDbContext context = _contextFactory.CreateDbContext())
            {
                Genre genre = context.Genres.FirstOrDefault(g => g.Name == genreName)!;
                context.Genres.Remove(genre);

                await context.SaveChangesAsync();
            }
        }
    }
}
