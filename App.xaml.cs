using Microsoft.EntityFrameworkCore;
using MovieLibrary.DbContexts;
using MovieLibrary.Models;
using MovieLibrary.Stores;
using MovieLibrary.ViewModels;
using System.IO;
using System.Windows;

namespace MovieLibrary
{
    public partial class App : Application
    {
        private readonly LibraryStore _libraryStore;
        private readonly NavigationStore _navigationStore;
        private readonly MovieLibraryDbContextFactory _contextFactory;

        public App()
        {
            _contextFactory = new MovieLibraryDbContextFactory();
            _libraryStore = new LibraryStore(new Library(_contextFactory));
            _navigationStore = new NavigationStore();

            _navigationStore.CurrenViewModel = new HomePageViewModel(_navigationStore, _libraryStore, _contextFactory);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            CheckForPictureDirectory();

            using (MovieLibraryDbContext dbContext = _contextFactory.CreateDbContext())
                dbContext.Database.Migrate();

            Window window = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };
            window.Show();

            base.OnStartup(e);
        }

        private void CheckForPictureDirectory()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\pics"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\pics");
        }
    }
}
