using MovieLibrary.Stores;
using System.Collections.ObjectModel;
using System.Windows;

namespace MovieLibrary.ViewModels
{
    class ItemCollectionViewerViewModelBase : ViewModelBase
    {
        protected readonly LibraryStore _libraryStore;
        protected readonly List<string> _filters;
        protected readonly List<string> _sorters;

        protected ObservableCollection<MovieViewModel> _visibleMovies;
        protected ObservableCollection<MovieViewModel>? _preFilterMovies = null;

        protected string _selectedFilter = "All";
        protected string _selectedSorter = "A-Z";

        public IEnumerable<MovieViewModel> VisibleMovies
        {
            get => _visibleMovies;
            set { _visibleMovies = new ObservableCollection<MovieViewModel>(value); }
        }

        public List<string> Filters => _filters;
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                FilterResults(value);
                SortResults(SelectedSorter);

                OnPropertyChanged(nameof(VisibleMovies));
            }
        }

        public List<string> Sorters => _sorters;
        public string SelectedSorter
        {
            get => _selectedSorter;
            set
            {
                _selectedSorter = value;
                SortResults(value);

                OnPropertyChanged(nameof(VisibleMovies));
            }
        }

        public ItemCollectionViewerViewModelBase(NavigationStore navigationStore, LibraryStore libraryStore)
        {
            _libraryStore = libraryStore;

            _filters = new List<string>() { "All" };
            _sorters = ["A-Z", "Z-A", "Date ↑", "Date ↓", "Score ↑", "Score ↓"];
        }

        protected void FilterResults(string filter)
        {
            if (filter == "All" && _preFilterMovies != null)
            {
                VisibleMovies = _preFilterMovies;
                _preFilterMovies = null;
            }
            else
            {
                if (_preFilterMovies == null)
                    _preFilterMovies = new ObservableCollection<MovieViewModel>(_visibleMovies);

                VisibleMovies = _preFilterMovies.Where((m) => m.GenreString.Contains(filter)).ToList();
            }
        }

        protected void SortResults(string sorter)
        {
            switch (sorter)
            {
                case "A-Z":
                    VisibleMovies = VisibleMovies.OrderBy(m => m.Title);
                    break;

                case "Z-A":
                    VisibleMovies = VisibleMovies.OrderByDescending(m => m.Title);
                    break;

                case "Date ↓":
                    try { VisibleMovies = VisibleMovies.OrderBy(m => DateOnly.Parse("1,1," + m.Date.Split("–")[0])); }
                    catch (FormatException) { MessageBox.Show("Invalid date format preventing sort operation"); }
                    break;

                case "Date ↑":
                    try { VisibleMovies = VisibleMovies.OrderByDescending(m => DateOnly.Parse("1,1," + m.Date.Split("–")[0])); }
                    catch (FormatException) { MessageBox.Show("Invalid date format preventing sort operation"); }
                    break;

                case "Score ↓":
                    VisibleMovies = VisibleMovies.OrderBy(m => m.Score);
                    break;

                case "Score ↑":
                    VisibleMovies = VisibleMovies.OrderByDescending(m => m.Score);
                    break;

            }
        }
    }
}
