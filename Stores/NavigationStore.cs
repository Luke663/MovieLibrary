using MovieLibrary.ViewModels;

namespace MovieLibrary.Stores
{
    internal class NavigationStore
    {
        public event Action? CurrentViewModelChanged;

        public ViewModelBase? savedPreviousPage = null;

        private ViewModelBase _currenViewModel;
        public ViewModelBase CurrenViewModel
        {
            get => _currenViewModel;
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
