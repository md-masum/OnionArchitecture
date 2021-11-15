using System;
using System.Collections.Generic;

namespace Core.Exceptions
{
    public class NotFoundException : Exception
    {
        private const string DefaultMessage = "Requested data not found.";
        public List<string> Errors { get; }


        public NotFoundException() : base(DefaultMessage)
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, List<string> errors) : base(message)
        {
            Errors = new List<string>();
            foreach (var error in errors)
            {
                Errors.Add(error);
            }
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
