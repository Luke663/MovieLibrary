using MovieLibrary.Stores;

namespace MovieLibrary.Commands
{
    class GoBackCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        public GoBackCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public override void Execute(object? parameter)
        {
            _navigationStore.CurrenViewModel = _navigationStore.savedPreviousPage!;
            _navigationStore.savedPreviousPage = null;
        }
    }
}