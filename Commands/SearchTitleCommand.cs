using MovieLibrary.DbContexts;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Commands
{
    class SearchTitleCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public SearchTitleCommand(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;
            _contextFactory = contextFactory;
        }

        public override void Execute(object? parameter)
        {
            if (parameter == null) // Error Title not found
                return;

            (List<MovieViewModel> movies, List<string> filters) = _libraryStore.GetMovieByTitle((string)parameter);

            _navigationStore.CurrenViewModel = new SearchPageViewModel(_navigationStore, _libraryStore, movies, filters, _contextFactory);
        }
    }
}
