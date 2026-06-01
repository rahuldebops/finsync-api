using finsyncapi.Models;
using System.Net;

namespace finsyncapi.Helper
{
    public static class ResponseHelper
    {
        public static Response<T> Success<T>(T result, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new Response<T>
            {
                Result = result,
                //StatusCode = statusCode,
                Message = message
            };
        }

        public static Response<T> Fail<T>(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string? errorCode = null, Dictionary<string, object>? metadata = null)
        {
            return new Response<T>
            {
                Result = default,
                //StatusCode = statusCode,
                Message = message,
                ErrorCode = errorCode,
                //Metadata = metadata
            };
        }

        // Optional: Non-generic variant for failures
        public static Response<object> Fail(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string? errorCode = null, Dictionary<string, object>? metadata = null)
        {
            return new Response<object>
            {
                Result = null,
                //StatusCode = statusCode,
                Message = message,
                ErrorCode = errorCode,
                //Metadata = metadata
            };
        }
    }

}
