using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Thargoid
    {
        [JsonPropertyName("TG_ENCOUNTER_KILLED")]
        public int EncounterKilled { get; init; }

        [JsonPropertyName("TG_ENCOUNTER_WAKES")]
        public int EncounterWakes { get; init; }

        [JsonPropertyName("TG_ENCOUNTER_TOTAL")]
        public int EncounterTotal { get; init; }

        [JsonPropertyName("TG_ENCOUNTER_TOTAL_LAST_SYSTEM")]
        public string LastSystem { get; init; }

        [JsonPropertyName("TG_ENCOUNTER_TOTAL_LAST_TIMESTAMP")]
        public string LastTimestamp { get; init; }

        [JsonPropertyName("TG_ENCOUNTER_TOTAL_LAST_SHIP")]
        public string LastShip { get; init; }
    }
}
