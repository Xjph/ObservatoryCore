using System.Text.Json;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Converters
{
    class MutableStringDoubleConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return reader.GetString();
            else
                return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            try
            {
                double asDbl = Convert.ToDouble(value);
                JsonSerializer.Serialize(writer, asDbl, options);
            }
            catch (Exception ex)
            {
                JsonSerializer.Serialize(writer, value?.ToString(), options);
            }
        }
    }
}
