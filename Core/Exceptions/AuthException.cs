using System;

namespace Core.Exceptions
{
    public class AuthException : Exception
    {
        private const string DefaultMessage = "An Error occured.";
        public int StatusCode { get; set; }


        public AuthException() : base(DefaultMessage)
        {
            StatusCode = 500;
        }

        public AuthException(string message) : base(message)
        {
            StatusCode = 500;
        }
        public AuthException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        public AuthException(string message, int statusCode, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
