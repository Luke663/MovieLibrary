using Microsoft.EntityFrameworkCore;
using MovieLibrary.Models;

namespace MovieLibrary.DbContexts
{
    internal class MovieLibraryDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public MovieLibraryDbContext(DbContextOptions options) : base(options) { }
    }
}
