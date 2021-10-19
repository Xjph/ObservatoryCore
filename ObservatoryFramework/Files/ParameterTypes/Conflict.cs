using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Conflict
    {
        public string WarType { get; init; }

        public string Status { get; init; }

        [JsonPropertyName("Faction1")]
        public WarFaction FirstFaction { get; init; }

        [JsonPropertyName("Faction2")]
        public WarFaction SecondFaction { get; init; }
    }
}
