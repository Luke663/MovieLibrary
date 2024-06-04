using MovieLibrary.DbContexts;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;
using System.Windows;

namespace MovieLibrary.Commands
{
    class ViewMovieCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public ViewMovieCommand(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;
            _contextFactory = contextFactory;
        }

        public override void Execute(object? parameter)
        {
            MovieViewModel movie = _libraryStore.GetSingleEntry((int?)parameter);

            if (movie.Title == "")
            {
                MessageBox.Show("There are no entries available to view.");
                return;
            }

            // Save current state for use of the 'Back' button
            _navigationStore.savedPreviousPage = _navigationStore.CurrenViewModel;

            _navigationStore.CurrenViewModel = new MoviePageViewModel(movie, _navigationStore, _libraryStore, _contextFactory);
        }
    }
}
