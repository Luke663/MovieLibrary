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
            if (parameter == null) // Error no Movie found
                return;

            NoteUpdateService service = new NoteUpdateService(_contextFactory);
            service.UpdateMovie((MovieViewModel)parameter);

            _libraryStore.UpdateAfterNoteAlteration((MovieViewModel)parameter);
        }
    }
}
