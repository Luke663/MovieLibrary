using MovieLibrary.DbContexts;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;
using System.Windows;

namespace MovieLibrary.Commands
{
    // Finds all movies matching the selected genre.

    class SearchForGenreCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public SearchForGenreCommand(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;
            _contextFactory = contextFactory;
        }

        public override void Execute(object? parameter)
        {
            if (parameter == null || (string)parameter == "")
            {
                MessageBox.Show("Error! Genre name not found.");
                return;
            }

            (List<MovieViewModel> movies, List<string> filters) = _libraryStore.GetMoviesByGenre((string)parameter);

            _navigationStore.CurrenViewModel = new SearchPageViewModel(_navigationStore, _libraryStore, movies, filters, _contextFactory);
        }
    }
}
