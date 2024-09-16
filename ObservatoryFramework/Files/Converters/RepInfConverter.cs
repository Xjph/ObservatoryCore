using System.Text.Json;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Converters
{
    public class RepInfConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString().Trim().Length;
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
