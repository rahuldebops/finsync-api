using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace finsyncapi.DTO
{
    public class DbFunctionResultDto<T>
    {
        [JsonPropertyName("data")]
        //[JsonConverter(typeof(DbFunctionDataConverterFactory))]
        public T? Data { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }
    }

/*    public class DbFunctionDataConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => true;

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == typeof(long))
            {
                return new LongDataConverter();
            }
            throw new NotSupportedException($"Type {typeToConvert} is not supported by DbFunctionDataConverterFactory.");
        }
    }

    public class LongDataConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt64();
            }
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = System.Text.Json.JsonDocument.ParseValue(ref reader);
                if (doc.RootElement.TryGetProperty("id", out var idProp))
                {
                    return idProp.GetInt64();
                }
            }
            return 0;
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
*/
}
