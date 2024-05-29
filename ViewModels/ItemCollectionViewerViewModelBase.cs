using MovieLibrary.Stores;
using System.Collections.ObjectModel;
using System.Windows;

namespace MovieLibrary.ViewModels
{
    // Base class of the extracted commonalities between the 'View All' and 'Search Results' pages.

    class ItemCollectionViewerViewModelBase : ViewModelBase
    {
        protected readonly LibraryStore _libraryStore;
        protected readonly List<string> _filters;
        protected readonly List<string> _sorters;

        protected ObservableCollection<MovieViewModel> _visibleMovies = new ObservableCollection<MovieViewModel>();
        protected ObservableCollection<MovieViewModel>? _preFilterMovies = null;

        // Initial values displayed in dropdown boxes
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

        public ItemCollectionViewerViewModelBase(LibraryStore libraryStore)
        {
            _libraryStore = libraryStore;

            // Initialise sort and filter dropdowns with miminum required fields
            _filters = new List<string>() { "All" };
            _sorters = ["A-Z", "Z-A", "Date ↑", "Date ↓", "Score ↑", "Score ↓"];
        }

        /// <summary>
        /// Sets the currently visible movies to be either a subset of the full range of entries (all
        /// for the 'View All' page or a set of search results for the 'Search Results' page) or the full set.
        /// </summary>
        /// <param name="filter">Parameter by which movies are filtered.</param>
        protected void FilterResults(string filter)
        {
            if (filter == "All" && _preFilterMovies != null) // Switching back from a filtered set to all
            {
                VisibleMovies = _preFilterMovies;
                _preFilterMovies = null;
            }
            else
            {
                if (_preFilterMovies == null) // Switching from all entries to a filtered set
                    _preFilterMovies = new ObservableCollection<MovieViewModel>(_visibleMovies);

                // Sets VisibleMovies to a subset of the full range of values
                VisibleMovies = _preFilterMovies.Where((m) => m.GenreString.Contains(filter)).ToList();
            }
        }

        /// <summary>
        /// Rearranges the set of currently visible movies via the selected property.
        /// </summary>
        /// <param name="sorter">Selected property to filter entries by (Date, title...).</param>
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
