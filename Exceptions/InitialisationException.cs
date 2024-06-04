namespace MovieLibrary.Exceptions
{
    internal class InitialisationException : Exception
    {
        private readonly string _exceptionType;
        public string ExceptionType => _exceptionType;

        public InitialisationException(string message)
        {
            _exceptionType = message;
        }
    }
}
