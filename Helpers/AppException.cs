using System.Net;

namespace finsyncapi.Helper
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string? ErrorCode { get; }
        public Dictionary<string, object>? Metadata { get; }

        public AppException(
            string? message = null,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            string? errorCode = null,
            Dictionary<string, object>? metadata = null
        ) : base(message ?? Messages.SomethingWentWrong)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Metadata = metadata;
        }
    }
}
