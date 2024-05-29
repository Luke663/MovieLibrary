using HtmlAgilityPack;
using System.Configuration;
using System.Net;
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
        }

        public string ExtractDuration(string date_rating_duration)
        {
            Regex r = new Regex(@"([0-9]h\s)?([0-9]{1,2}m)"); // Find mins (42m) or both (1h 42m)
            Regex r2 = new Regex(@"([0-9]h)"); // Find hours only (2h)

            string retString = r.Match(date_rating_duration).ToString();

            return retString == "" ? r2.Match(date_rating_duration).ToString() : retString;
        }

        /// <summary>
        /// Retrieves the entry's release date (for simplification this needs to be done
        /// after duration has been removed from the combined date_rating_duration string).
        /// </summary>
        public string ExtractDate(string date_rating)
        {
            Regex r = new Regex(@"[0-9]{4}–?(\s)?([0-9]{4})?");

            return r.Match(date_rating).ToString();
        }

        public string ExtractGenres(string genreText, ref List<string> genres)
        {
            string dbText = "";
            string[] alternates = new string[] { "Sci-Fi", "Film-Noir", "Reality-TV", "Game-Show" };

            // To simplify parsing this looks for known variations on the typical capital letter folowed by lower case.
            foreach (string alt in alternates)
            {
                if (genreText.Contains(alt))
                {
                    dbText = dbText + alt + ", ";
                    genreText = genreText.Replace(alt, "");

                    genres.Add(alt);
                }
            }

            // Regex to extract each genre (eg Action) adding them to a list for individual processing
            // and to the entry's own (string) record.
            Regex r = new Regex(@"[A-Z][a-z]+");
            MatchCollection matches = r.Matches(genreText);

            foreach (Match match in matches)
            {
                genres.Add(match.ToString());
                dbText = dbText + match.ToString() + ", ";
            }

            return dbText.Trim(new char[] { ' ', ',' });
        }

        public HtmlDocument? GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();

            try { return web.Load(url); }
            catch (UriFormatException) { return null; }
        }

        /// <summary>
        /// Retrieves the desired text from the given document via the given X-Path.
        /// (replacing characters as needed due to HtmlAgilityPack's formatting)
        /// </summary>
        public string ExtractText(HtmlDocument document, string xPath)
        {
            if (document.DocumentNode.SelectSingleNode(xPath) != null)
                return RemoveFormattingErrors(document.DocumentNode.SelectSingleNode(xPath).InnerText);

            return "";
        }

        private string RemoveFormattingErrors(string text)
        {
            return text.Replace("&quot;", "\"").Replace("&#039;", "'").Replace("&mdash;", " - ").Replace("&iuml;", "i").Replace("&#x27;", "'");
        }

        public string ExtractImageURL(HtmlDocument document, string xPath)
        {
            return document.DocumentNode.SelectSingleNode(xPath).Attributes["src"].Value;
        }

        /// <summary>
        /// Save image file to 'pics' folder, removing invalid characters where needed.
        /// </summary>
        public string DownloadImage(string imageURL, string entry_title)
        {
            string imagePath = "pics/" + ValidateFileName(entry_title) + ".jpg";

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(imageURL, imagePath);
            }

            return imagePath;
        }

        private string ValidateFileName(string filname)
        {
            char[] restrictedCharacters = new char[] { '<', '>', ':', '\"', '/', '\\', '|', '?', '*' };

            foreach (char character in restrictedCharacters)
                filname = filname.Replace(character, '-');

            return filname;
        }
    }
}
