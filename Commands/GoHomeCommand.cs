using MovieLibrary.DbContexts;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Commands
{
    class GoHomeCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public GoHomeCommand(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;
            _contextFactory = contextFactory;
        }

        public override void Execute(object? parameter)
        {
            _navigationStore.CurrenViewModel = new HomePageViewModel(_navigationStore, _libraryStore, _contextFactory);
        }
    }
}
