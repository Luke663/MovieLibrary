using MovieLibrary.DbContexts;
using MovieLibrary.Services;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Commands
{
    // Called to scrape a movie and insert it into the database and library

    class AddMovieCommand : CommandBase
    {
        private readonly MovieLibraryDbContextFactory _contextFactory;
        private readonly NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;

        public AddMovieCommand(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;
        }

        public override void Execute(object? parameter)
        {
            if (parameter == null) // Error no web address found
                return;

            // Scrape and insert movie into database and update library in memory
            AddMovieService service = new AddMovieService(_contextFactory, _libraryStore);
            MovieViewModel? movie = service.AddMovie((string)parameter).Result;

            if (movie == null) // Error insertion failed
                return;

            // Save current state for use of the 'Back' button
            _navigationStore.savedPreviousPage = _navigationStore.CurrenViewModel;

            // Switch to view inserted movie
            _navigationStore.CurrenViewModel = new MoviePageViewModel(movie, _navigationStore, _libraryStore, _contextFactory);
        }
    }
}
