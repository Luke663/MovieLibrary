using MovieLibrary.ViewModels;

namespace MovieLibrary.Stores
{
    internal class NavigationStore
    {
        public event Action? CurrentViewModelChanged;

        // Store for a previous page to allow the persistence of search results or filter/sort operations when
        // switching between the 'View All' or 'Search Results' pages and the 'View Entry Page' (MoviePageView).
        public ViewModelBase? savedPreviousPage = null;

        private ViewModelBase? _currenViewModel;
        public ViewModelBase CurrenViewModel
        {
            get => _currenViewModel!;
            set
            {
                _currenViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
