using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                services.AddSingleton<MovieLibraryDbContextFactory>();

                services.AddSingleton((s) => new Library(s.GetRequiredService<MovieLibraryDbContextFactory>()));
                services.AddSingleton((s) => new LibraryStore(s.GetRequiredService<Library>()));

                services.AddSingleton<NavigationStore>();

                services.AddSingleton((s) => new MainViewModel(s.GetRequiredService<NavigationStore>()));
                services.AddSingleton((s) => new MainWindow()
                {
                    DataContext = s.GetRequiredService<MainViewModel>()
                });
            }
            ).Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            CheckForPictureDirectory();

            _host.Start();

            using (MovieLibraryDbContext dbContext = _host.Services.GetRequiredService<MovieLibraryDbContextFactory>().CreateDbContext())
                dbContext.Database.Migrate();

            // Instatiate Home page
            NavigationStore navigation = _host.Services.GetRequiredService<NavigationStore>();
            navigation.CurrenViewModel = new HomePageViewModel(
                _host.Services.GetRequiredService<NavigationStore>(),
                _host.Services.GetRequiredService<LibraryStore>(),
                _host.Services.GetRequiredService<MovieLibraryDbContextFactory>());

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();

            base.OnExit(e);
        }

        /// <summary>
        /// Ensures the picture directory exits.
        /// </summary>
        private void CheckForPictureDirectory()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\pics"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\pics");
        }
    }
}
