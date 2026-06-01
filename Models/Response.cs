using System.Net;

namespace finsyncapi.Models
{
    public class Response<T>
    {
        public T? Result { get; set; }
        //public HttpStatusCode? StatusCode { get; set; } = HttpStatusCode.InternalServerError;
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }  // Optional short code like "INVALID_INPUT"
        //public Dictionary<string, object>? Metadata { get; set; }  // Optional context info
    }

    //public class ResultDto<T>
    //{
    //    public T? Data { get; set; }
    //    public string? Message { get; set; }
    //    public bool Success { get; set; }  
    //}
}
