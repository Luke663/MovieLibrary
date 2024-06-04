namespace MovieLibrary.Exceptions
{
    internal class ScrapeFailedException : Exception
    {
        private readonly string _failedProperty;
        private readonly string _xpath;

        public string Failure => _failedProperty;
        public string Xpath => _xpath;

        public ScrapeFailedException(string failedProperty, string xpath)
        {
            _failedProperty = failedProperty;
            _xpath = xpath;
        }
    }
}
