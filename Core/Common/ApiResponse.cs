using System.Collections.Generic;

namespace Core.Common
{
    public class ApiResponse<T>
    {
        public ApiResponse()
        {
            
        }

        public ApiResponse(string message)
        {
            IsSuccess = false;
            Message = message;
        }

        public ApiResponse(List<string> errors, string message = null)
        {
            IsSuccess = false;
            Message = message;
            Errors = errors;
        }

        public ApiResponse(string message, List<string> innerExceptions)
        {
            IsSuccess = false;
            Message = message;
            InnerExceptions = innerExceptions;
        }

        public ApiResponse(T data, string message = null)
        {
            IsSuccess = true;
            Message = message;
            Data = data;
        }

        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public string Message { get; set; }
        public List<string> InnerExceptions { get; set; }
    }
}
