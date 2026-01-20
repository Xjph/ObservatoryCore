using System.Text.Json;
using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Converters
{

    /// <summary>
    /// Faction changed from a simple string to an object with additional state information. If we find a string convert it to an object with null state.
    /// </summary>
    public class LegacyFactionConverter<TFaction> : JsonConverter<TFaction> where TFaction : Faction, new()
    {
        public override TFaction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new TFaction { Name = reader.GetString(), FactionState = null };
            }
            else
            {
                return JsonSerializer.Deserialize<TFaction>(ref reader);
            }
        }

        public override void Write(Utf8JsonWriter writer, TFaction value, JsonSerializerOptions options)
        {
            if (value != null)
                JsonSerializer.Serialize(writer, value, options);
        }
    }
}
