using MovieLibrary.Commands;
using MovieLibrary.DbContexts;
using MovieLibrary.Models;
using MovieLibrary.Stores;
using System.Windows.Input;

namespace MovieLibrary.ViewModels
{
    class HomePageViewModel : ViewModelBase
    {
        public NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;

        public ICommand? SearchTitleCommand { get; }
        public ICommand? ViewGenreCommand { get; }
        public ICommand? GoToViewAllCommand { get; }
        public ICommand? ViewEntryCommand { get; }
        public ICommand? AddMovieCommand { get; }

        public MovieViewModel RandomMovie { get; set; }
        public List<Genre>? GenreList { get; }

        public HomePageViewModel(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;

            GoToViewAllCommand = new GoToViewAllCommand(navigationStore, libraryStore, contextFactory);
            SearchTitleCommand = new SearchTitleCommand(navigationStore, libraryStore, contextFactory);
            ViewGenreCommand = new SearchForGenreCommand(navigationStore, libraryStore, contextFactory);
            ViewEntryCommand = new ViewMovieCommand(navigationStore, libraryStore, contextFactory);
            AddMovieCommand = new AddMovieCommand(navigationStore, libraryStore, contextFactory);

            RandomMovie = _libraryStore.GetSingleEntry();
            GenreList = _libraryStore.Genres.OrderBy(g => g.Name).ToList();
        }
    }
}
