using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using finsyncapi.Helpers;

namespace finsyncapi.Models
{
    [TypeConverter(typeof(EncodedIdTypeConverter))]
    public readonly struct SnowFlakeId
    {
        public long Value { get; }

        public SnowFlakeId(long value) => Value = value;
        public static SnowFlakeId Parse(string encoded) => new(Base62Converter.Decode(encoded));
        public override string ToString() => Base62Converter.Encode(Value);

        public static implicit operator long(SnowFlakeId id) => id.Value;
        //public static explicit operator SnowFlakeId(long value) => new(value);
        public static implicit operator SnowFlakeId(long value) => new(value);
    }

    public class EncodedIdJsonConverter : JsonConverter<SnowFlakeId>
    {
        public override SnowFlakeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var encoded = reader.GetString();
            return SnowFlakeId.Parse(encoded);
        }

        public override void Write(Utf8JsonWriter writer, SnowFlakeId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
            //writer.WriteNumberValue(value.Value);
        }
    }
    public class RawSnowFlakeIdJsonConverter : JsonConverter<SnowFlakeId>
    {
        public override SnowFlakeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new SnowFlakeId(reader.GetInt64());

        public override void Write(Utf8JsonWriter writer, SnowFlakeId value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.Value);
    }

    public class EncodedIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            SnowFlakeId.Parse((string)value);
    }

}
