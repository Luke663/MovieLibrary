namespace MovieLibrary.Exceptions
{
    internal class XpathNotFoundException : Exception
    {
        private readonly string _xpathName;
        public string XpathName => _xpathName;

        public XpathNotFoundException(string xpathName)
        {
            _xpathName = xpathName;
        }
    }
}
