using MovieLibrary.DbContexts;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Commands
{
    class GoToViewAllCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public GoToViewAllCommand(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;
            _contextFactory = contextFactory;
        }

        public override void Execute(object? parameter)
        {
            _navigationStore.CurrenViewModel = new ViewAllPageViewModel(_navigationStore, _libraryStore, _contextFactory);
        }
    }
}
