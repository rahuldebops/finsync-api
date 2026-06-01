using finsyncapi.Helper;
using finsyncapi.Models;
using System.Net;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IConfiguration _config;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IConfiguration config)
        {
            _next = next;
            _logger = logger;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");

                HttpStatusCode statusCode;
                string message;
                string? errorCode = null;
                Dictionary<string, object>? metadata = null;

                string env = _config.GetValue<string>("AppEnvironment") ?? AppEnvironment.PROD.ToString();
                bool showDetails = env != AppEnvironment.PROD.ToString();

                if (ex is AppException appEx)
                {
                    statusCode = appEx.StatusCode;
                    message = appEx.Message;
                    errorCode = appEx.ErrorCode;
                    metadata = appEx.Metadata;
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                    message = showDetails ? ex.ToString() : Messages.SomethingWentWrong;
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var response = new Response<object>
                {
                    Result = null,
                    Message = message,
                    ErrorCode = errorCode
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
