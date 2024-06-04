using MovieLibrary.Commands;
using MovieLibrary.DbContexts;
using MovieLibrary.Stores;
using System.Windows.Input;

namespace MovieLibrary.ViewModels
{
    class SearchPageViewModel : ItemCollectionViewerViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand ViewMovieCommand { get; }

        public SearchPageViewModel(NavigationStore navigationStore, LibraryStore libraryStore, List<MovieViewModel> results,
            List<string> filters, MovieLibraryDbContextFactory contextFactory) : base(libraryStore)
        {
            NavigateHomeCommand = new GoHomeCommand(navigationStore, libraryStore, contextFactory);
            ViewMovieCommand = new ViewMovieCommand(navigationStore, libraryStore, contextFactory);

            _visibleMovies = [.. results.OrderBy(a => a.Title)];

            foreach (string filter in filters)
                _filters.Add(filter);
        }
    }
}
