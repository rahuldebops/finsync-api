using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace finsyncapi.Dto
{
    public class ResultDto<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
        [JsonPropertyName("success")]
        public bool Success { get; set; } = false;
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}