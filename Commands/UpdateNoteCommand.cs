using MovieLibrary.DbContexts;
using MovieLibrary.Services;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;

namespace MovieLibrary.Commands
{
    class UpdateNoteCommand : CommandBase
    {
        private readonly MovieLibraryDbContextFactory _contextFactory;
        private readonly LibraryStore _libraryStore;

        public UpdateNoteCommand(LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _libraryStore = libraryStore;
        }

        public override void Execute(object? parameter)
        {
            if (parameter == null) // Error no MovieViewModel found
                return;

            // Update database
            NoteUpdateService service = new NoteUpdateService(_contextFactory);
            _ = service.UpdateMovie((MovieViewModel)parameter);

            // Update library held in memory
            _libraryStore.UpdateAfterNoteAlteration((MovieViewModel)parameter);
        }
    }
}
