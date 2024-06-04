using MovieLibrary.DbContexts;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;
using System.Windows;

namespace MovieLibrary.Commands
{
    // Finds movie(s) and their relevant genres in the current library with titles matching/containing the given search term.

    class SearchTitleCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public SearchTitleCommand(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;
            _contextFactory = contextFactory;
        }

        public override void Execute(object? parameter)
        {
            if (parameter == null) // Error Title not found
            {
                MessageBox.Show("Error! Title is null.");
                return;
            }

            (List<MovieViewModel> movies, List<string> filters) = _libraryStore.GetMoviesByTitle((string)parameter);

            _navigationStore.CurrenViewModel = new SearchPageViewModel(_navigationStore, _libraryStore, movies, filters, _contextFactory);
        }
    }
}
