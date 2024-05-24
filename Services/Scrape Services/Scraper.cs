using HtmlAgilityPack;
using MovieLibrary.Models;

namespace MovieLibrary.Services.Scrape_Services
{
    class Scraper
    {
        private ScraperUtils _utils = new ScraperUtils();

        public Movie ScrapeMovie(string entry_url)
        {
            HtmlDocument document = _utils.GetDocument(entry_url);
            Movie returnMovie = new Movie();

            if (document == null || !entry_url.Contains("www.imdb.com"))
                return returnMovie;


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

            List<string> genres = new List<string>();
            string genres_text = _utils.ExtractText(document, _utils.XPaths["GENRES"]);

            returnMovie.GenreString = _utils.ExtractGenres(genres_text, ref genres);


            return returnMovie;
        }
    }
}
