using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Core.Common;
using Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core.Middleware
{
    internal class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                ApiResponse<object> errorResponse;
                var errors = FilterErrorMessage(ex);
                Console.WriteLine(ex);
                switch (ex)
                {
                    case CustomException exception:
                        errorResponse = new ApiResponse<object>(exception.Errors, exception.Message);
                        response.StatusCode = (int) HttpStatusCode.BadRequest;
                        break;
                    case NotFoundException exception:
                        errorResponse = new ApiResponse<object>(exception.Message, exception.Errors);
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case AuthException exception:
                        errorResponse = new ApiResponse<object>(ex.Message, errors);
                        response.StatusCode = exception.StatusCode;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        errorResponse = new ApiResponse<object>(ex.Message, errors);
                        break;
                }

                var settings = new JsonSerializerSettings { ContractResolver = new CamelCaseContractResolver() };

                var errorJson = JsonConvert.SerializeObject(errorResponse, Formatting.Indented, settings);

                await response.WriteAsync(errorJson);
            }
        }

        #region Supported Methods

        private static List<string> FilterErrorMessage(Exception ex)
        {
            var errors = new List<string>();
            var e = ex;
            while (e != null)
            {
                errors.Add(e.Message);
                e = e.InnerException;
            }

            return errors.Count > 1 ? errors : null;
        }

        public class CamelCaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return char.ToLower(propertyName[0]) + propertyName[1..];
            }
        }

        #endregion
    }

    public static class GlobalErrorHanding
    {
        public static void UseGlobalErrorHandlingMiddleware(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<GlobalErrorHandlingMiddleware>();
        }
    }
}
