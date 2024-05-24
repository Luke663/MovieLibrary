using MovieLibrary.DbContexts;
using MovieLibrary.Services;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Commands
{
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
            if (parameter == null) // Error no address found
                return;

            AddMovieService service = new AddMovieService(_contextFactory, _libraryStore);
            MovieViewModel movie = service.AddMovie((string)parameter).Result;

            if (movie == null) // Error insertion failed
                return;

            _navigationStore.CurrenViewModel = new MoviePageViewModel(movie, _navigationStore, _libraryStore, _contextFactory);
        }
    }
}
