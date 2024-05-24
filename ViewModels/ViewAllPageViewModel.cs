﻿using MovieLibrary.Commands;
using MovieLibrary.DbContexts;
using MovieLibrary.Models;
using MovieLibrary.Stores;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MovieLibrary.ViewModels
{
    class ViewAllPageViewModel : ItemCollectionViewerViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand ViewMovieCommand { get; }

        public ViewAllPageViewModel(NavigationStore navigationStore, LibraryStore libraryStore, MovieLibraryDbContextFactory contextFactory) : base(navigationStore, libraryStore)
        {
            NavigateHomeCommand = new GoHomeCommand(navigationStore, libraryStore, contextFactory);
            ViewMovieCommand = new ViewMovieCommand(navigationStore, libraryStore, contextFactory);

            _visibleMovies = new ObservableCollection<MovieViewModel>();

            foreach (Movie movie in _libraryStore.Movies)
                _visibleMovies.Add(new MovieViewModel(movie));

            foreach (Genre genre in _libraryStore.Genres)
                _filters.Add(genre.Name);

            SortResults(SelectedSorter);
        }
    }
}
