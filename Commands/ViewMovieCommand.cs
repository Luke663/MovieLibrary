using MovieLibrary.DbContexts;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Commands
{
    class ViewMovieCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public ViewMovieCommand(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;
            _contextFactory = contextFactory;
        }

        public override void Execute(object? parameter)
        {
            _navigationStore.savedPreviousPage = _navigationStore.CurrenViewModel;

            _navigationStore.CurrenViewModel = new MoviePageViewModel(_libraryStore.GetSingleEntry((string?)parameter), _navigationStore, _libraryStore, _contextFactory);
        }
    }
}
