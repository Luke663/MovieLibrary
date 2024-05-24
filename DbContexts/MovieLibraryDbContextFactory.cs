using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace MovieLibrary.DbContexts
{
    class MovieLibraryDbContextFactory : IDesignTimeDbContextFactory<MovieLibraryDbContext>
    {
        public MovieLibraryDbContext CreateDbContext(string[]? args = null)
        {
            string? connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder<MovieLibraryDbContext>().UseSqlite(connectionString);

            return new MovieLibraryDbContext(builder.Options);
        }
    }
}
