using MovieLibrary.Commands;
using MovieLibrary.DbContexts;
using MovieLibrary.Stores;
using System.Windows.Input;

namespace MovieLibrary.ViewModels
{
    class MoviePageViewModel : ViewModelBase
    {
        private readonly MovieViewModel _movieToView;
        public MovieViewModel MovieToView => _movieToView;

        public ICommand NavigateHomeCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand DeleteMovieCommand { get; }
        public ICommand UpdateNoteCommand { get; }

        public int Id => _movieToView.Id;
        public string Title => _movieToView.Title;
        public string Description => _movieToView.Description;
        public string Path => _movieToView.Path;
        public string GenreString => _movieToView.GenreString;
        public string Date => _movieToView.Date;
        public string Duration => _movieToView.Duration;
        public string Score => _movieToView.Score;
        public string AgeRating => _movieToView.AgeRating;

        public string Note
        {
            get => _movieToView.Note;
            set
            {
                _movieToView.Note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        public MoviePageViewModel(MovieViewModel movieToView, NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory)
        {
            _movieToView = movieToView;

            NavigateHomeCommand = new GoHomeCommand(navigationStore, libraryStore, contextFactory);
            GoBackCommand = new GoBackCommand(navigationStore);
            DeleteMovieCommand = new DeleteMovieCommand(navigationStore, libraryStore, contextFactory);
            UpdateNoteCommand = new UpdateNoteCommand(libraryStore, contextFactory);
        }
    }
}
