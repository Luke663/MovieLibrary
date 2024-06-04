using MovieLibrary.DbContexts;
using MovieLibrary.Services;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;
using System.Windows;

namespace MovieLibrary.Commands
{
    class DeleteMovieCommand : CommandBase
    {
        private readonly MovieLibraryDbContextFactory _contextFactory;
        private readonly NavigationStore _navigationStore;
        private readonly LibraryStore _libraryStore;

        public DeleteMovieCommand(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _navigationStore = navigationStore;
            _libraryStore = libraryStore;
        }

        public override void Execute(object? parameter)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this entry?\nThis cannot be undone.",
                "Confirm Deletion", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.No)
                return;

            if (parameter == null)
            {
                MessageBox.Show("Error! Entry.Id not found for delete operation.");
                return;
            }

            // Update database
            DeletionService service = new DeletionService(_contextFactory);
            service.DeleteMovie((int)parameter);

            // Update library held in memory
            _libraryStore.UpdateStoreAfterDeletion((int)parameter);

            // Switch back to Home page
            _navigationStore.CurrenViewModel = new HomePageViewModel(_navigationStore, _libraryStore, _contextFactory);
        }
    }
}
