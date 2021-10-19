using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Crew
    {
        [JsonPropertyName("NpcCrew_TotalWages")]
        public long NpcCrewTotalWages { get; init; }

        [JsonPropertyName("NpcCrew_Hired")]
        public int NpcCrewHired { get; init; }

        [JsonPropertyName("NpcCrew_Fired")]
        public int NpcCrewFired { get; init; }

        [JsonPropertyName("NpcCrew_Died")]
        public int NpcCrewDied { get; init; }
    }
}
