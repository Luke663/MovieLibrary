using HtmlAgilityPack;
using MovieLibrary.Exceptions;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MovieLibrary.Services.Scrape_Services
{
    class ScraperUtils
    {
        public readonly Dictionary<string, string> XPaths;

        public ScraperUtils()
        {
            XPaths = new Dictionary<string, string>
            {
                ["TITLE"] = ConfigurationManager.AppSettings.Get("TITLE")!,

                // returns: year, rating, duration (begins with TV Series for series) without spaces
                // Gives 3 or 4 items in one string, form of: "TV Series 1997–2003 12 44m" for series and "2023 15 1h 47m" for movies
                ["SECONDARY"] = ConfigurationManager.AppSettings.Get("SECONDARY")!,

                ["SCORE"] = ConfigurationManager.AppSettings.Get("SCORE")!,

                ["GENRES"] = ConfigurationManager.AppSettings.Get("GENRES")!,

                ["IMAGE"] = ConfigurationManager.AppSettings.Get("IMAGE")!,

                ["DESCRIPTION"] = ConfigurationManager.AppSettings.Get("DESCRIPTION")!,

                ["EPISODES"] = ConfigurationManager.AppSettings.Get("EPISODES")!,
            };

            foreach (string key in XPaths.Keys)
                if (XPaths[key] == null)
                    throw new XpathNotFoundException(key);
        }

        /// <summary>
        /// Retrieves the entry's duration from the given string.
        /// </summary>
        public string ExtractDuration(string date_rating_duration)
        {
            Regex r = new Regex(@"([0-9]h\s)?([0-9]{1,2}m)"); // Find mins (42m) or both (1h 42m)
            Regex r2 = new Regex(@"([0-9]h)"); // Find hours only (2h)

            string retString = r.Match(date_rating_duration).ToString();

            return retString == "" ? r2.Match(date_rating_duration).ToString() : retString;
        }

        /// <summary>
        /// Retrieves the entry's release date from the given string.
        /// </summary>
        public string ExtractDate(string date_rating)
        {
            Regex r = new Regex(@"[0-9]{4}–?(\s)?([0-9]{4})?");

            return r.Match(date_rating).ToString();
        }

        /// <summary>
        /// Splits the scraped text (e.g. ActionAdventureMysterySci-Fi) into its
        /// appropriate form (e.g. Action, Adventure, Mystery, Sci-Fi).
        /// </summary>
        /// <param name="genreText">(string) Scraped text.</param>
        /// <returns>Comma speperated string of genres.</returns>
        public string ExtractGenres(string genreText)
        {
            string dbText = "";
            string[] alternates = new string[] { "Sci-Fi", "Film-Noir", "Reality-TV", "Game-Show" };

            // To simplify parsing this looks for known variations on the typical
            // capital letter folowed by lower case letters caught by the regex below.
            foreach (string alt in alternates)
            {
                if (genreText.Contains(alt))
                {
                    dbText = dbText + alt + ", ";
                    genreText = genreText.Replace(alt, "");
                }
            }

            // Regex to extract each genre from the passed text (e.g. ActionAdventureMystery)
            MatchCollection matches = new Regex(@"[A-Z][a-z]+").Matches(genreText);

            foreach (Match match in matches)
                dbText = dbText + match.ToString() + ", ";

            return dbText.Trim(new char[] { ' ', ',' });
        }

        public HtmlDocument? GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();

            try { return web.Load(url); }
            catch (UriFormatException) { return null; }
        }

        /// <summary>
        /// Retrieves text from the given document via the given X-Path.
        /// </summary>
        public string ExtractText(HtmlDocument document, string xPath)
        {
            if (document.DocumentNode.SelectSingleNode(xPath) != null)
                return RemoveFormattingErrors(document.DocumentNode.SelectSingleNode(xPath).InnerText);

            return "";
        }

        /// <summary>
        /// Removes formatting errors created by reading the web page's text from the passed string.
        /// </summary>
        private string RemoveFormattingErrors(string text)
        {
            return text.Replace("&quot;", "\"").Replace("&#039;", "'").Replace("&mdash;", " - ").Replace("&iuml;", "i").Replace("&#x27;", "'");
        }

        /// <summary>
        /// Finds the specific URL for the movie's image.
        /// </summary>
        public string ExtractImageURL(HtmlDocument document, string xPath)
        {
            return document.DocumentNode.SelectSingleNode(xPath).Attributes["src"].Value;
        }

        /// <summary>
        /// Save image file to 'pics' folder.
        /// </summary>
        public string DownloadImage(string imageURL, string entry_title)
        {
            string imagePath = "pics/" + ValidateFileName(entry_title) + ".jpg";

            if (File.Exists(imagePath))
                imagePath = ResolveFilenameConflict(imagePath);

            try
            {
                using (var client = new HttpClient())
                    using (var s = client.GetStreamAsync(imageURL))
                        using (var fs = new FileStream(imagePath, FileMode.OpenOrCreate))
                            s.Result.CopyTo(fs);
            }
            catch (HttpRequestException) { return ""; }
            catch (UriFormatException) { return ""; }
            catch (IOException) { return ""; }

            return imagePath;
        }

        /// <summary>
        /// Append bracketted numbers to filename to resolve conflicts for
        /// movies with the same name.
        /// </summary>
        /// /// <returns>(string) Unique filename.</returns>
        private string ResolveFilenameConflict(string filename)
        {
            int counter = 2;
            filename = filename.Replace(".jpg", "");

            while (File.Exists(filename + "(" + counter + ").jpg"))
                counter++;

            return filename + "(" + counter + ").jpg";
        }

        /// <summary>
        /// Removes invalid characters from the image filename.
        /// </summary>
        /// <param name="filname">Filename to check for invalid characters.</param>
        /// <returns>(string) Sanitised filename.</returns>
        private string ValidateFileName(string filname)
        {
            char[] restrictedCharacters = new char[] { '<', '>', ':', '\"', '/', '\\', '|', '?', '*' };

            foreach (char character in restrictedCharacters)
                filname = filname.Replace(character, '-');

            return filname;
        }
    }
}
