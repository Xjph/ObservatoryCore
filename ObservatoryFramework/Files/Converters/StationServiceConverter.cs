using System.Text.Json;
using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Converters
{
    [Obsolete("Replaced by JsonStringEnumMemberConverter which can handle unkonwn values.")]
    public class StationServiceConverter : JsonConverter<StationService>
    {
        public override StationService Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            StationService services = StationService.None;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                services |= (StationService)Enum.Parse(typeof(StationService), reader.GetString(), true);
            }

            return services;

        }

        public override void Write(Utf8JsonWriter writer, StationService value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
