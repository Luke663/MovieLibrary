using HtmlAgilityPack;
using MovieLibrary.Exceptions;
using MovieLibrary.Models;
using System.Windows;

namespace MovieLibrary.Services.Scrape_Services
{
    class Scraper
    {
        private ScraperUtils _utils = new ScraperUtils();

        public Movie? ScrapeMovie(string entry_url)
        {
            ScraperUtils? utils = GetUtils();

            if (utils == null)
                return null;

            HtmlDocument? document = utils.GetDocument(entry_url)!;
            Movie returnMovie = new Movie();

            if (!URL_IsValid(document, entry_url))
                return null;


            returnMovie.Title = _utils.ExtractText(document, _utils.XPaths["TITLE"]);
            returnMovie.Path = _utils.DownloadImage(_utils.ExtractImageURL(document, _utils.XPaths["IMAGE"]), returnMovie.Title);
            returnMovie.Description = _utils.ExtractText(document, _utils.XPaths["DESCRIPTION"]);
            returnMovie.Score = _utils.ExtractText(document, _utils.XPaths["SCORE"]);

            // The date, rating and duration are displayed together so must be collected as a single string ("1994U1h 28m")
            string date_rating_duration = _utils.ExtractText(document, _utils.XPaths["SECONDARY"]);
            string duration = _utils.ExtractDuration(date_rating_duration);

            returnMovie.Duration = "Duration: " + duration;
            date_rating_duration = date_rating_duration.Replace(duration, "");

            returnMovie.Date = _utils.ExtractDate(date_rating_duration);
            date_rating_duration = date_rating_duration.Replace(returnMovie.Date, "");

            // Check for 'TV' in string meaning entry is a series. If so this replaces the duration text (Duration: 1h 24m)
            // with episode text (Episodes: 24).
            if (date_rating_duration.Contains("TV"))
            {
                date_rating_duration = date_rating_duration.Replace("TV Series", "").Replace("TV Mini Series", "");
                returnMovie.Duration = "Episodes: " + _utils.ExtractText(document, _utils.XPaths["EPISODES"]);
            }

            returnMovie.AgeRating = date_rating_duration;

            returnMovie.GenreString = _utils.ExtractGenres(_utils.ExtractText(document, _utils.XPaths["GENRES"]));


            CheckScrapeSuccessful(returnMovie);


            return returnMovie;
        }

        private ScraperUtils? GetUtils()
        {
            ScraperUtils utils;

            try
            {
                utils = new ScraperUtils();
            }
            catch (XpathNotFoundException ex)
            {
                MessageBox.Show("Failed to find Xpath value,\nMissing Xpath name: " + ex.XpathName);
                return null;
            }

            return utils;
        }

        private bool URL_IsValid(HtmlDocument? document, string movieURL)
        {
            if (document == null || !movieURL.Contains("www.imdb.com"))
            {
                MessageBox.Show($"Invalid address!\nValid web address = {document != null}\nValid movie page = {movieURL.Contains("www.imdb.com")}");
                return false;
            }

            return true;
        }

        private void CheckScrapeSuccessful(Movie movie)
        {
            if (movie.Title == "")
                throw new ScrapeFailedException("Title", "TITLE");

            if (movie.Path == "")
                throw new ScrapeFailedException("Image path", "IMAGE");

            if (movie.Description == "")
                throw new ScrapeFailedException("Description", "DESCRIPTION");

            if (movie.Score == "")
                throw new ScrapeFailedException("Score", "SCORE");

            if (movie.Date == "")
                throw new ScrapeFailedException("Date", "SECONDARY");

            if (movie.GenreString == "")
                throw new ScrapeFailedException("GenreString", "GENRES");

            if (movie.Duration.Contains("Duration: ") && movie.Duration.Replace("Duration: ", "") == "")
                throw new ScrapeFailedException("Duration", "SECONDARY");

            if (movie.Duration.Contains("Episodes: ") && movie.Duration.Replace("Episodes: ", "") == "")
                throw new ScrapeFailedException("Duration", "EPISODES");

            if (movie.AgeRating == "")
                throw new ScrapeFailedException("AgeRating", "SECONDARY");
        }
    }
}
