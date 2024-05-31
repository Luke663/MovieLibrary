using MovieLibrary.Stores;

namespace MovieLibrary.Commands
{
    // Called by MoviePageView's 'Back' button to switch back to either the 'View All' or 'Search Results' pages
    // allowing the persistance of a search and/or filter or sort operations.

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