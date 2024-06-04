using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using MovieLibrary.Exceptions;
using System.IO;

namespace MovieLibrary.DbContexts
{
    class MovieLibraryDbContextFactory : IDesignTimeDbContextFactory<MovieLibraryDbContext>
    {
        public MovieLibraryDbContext CreateDbContext(string[]? args = null)
        {
            if (!File.Exists("MovieLibrary.dll.config"))
                throw new InitialisationException("Error! Failed to find configuration file.\nFilename: MovieLibrary.dll.config.");

            string? connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

            if (connectionString == null || connectionString == "")
                throw new InitialisationException("Error! Database connection string not found in\nfile: MovieLibrary.dll.config.");

            DbContextOptionsBuilder builder = new DbContextOptionsBuilder<MovieLibraryDbContext>().UseSqlite(connectionString);

            return new MovieLibraryDbContext(builder.Options);
        }
    }
}
