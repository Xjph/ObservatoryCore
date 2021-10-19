using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Passengers
    {
        [JsonPropertyName("Passengers_Missions_Accepted")]
        public int MissionsAccepted { get; init; }

        [JsonPropertyName("Passengers_Missions_Disgruntled")]
        public int MissionsDisgruntled { get; init; }

        [JsonPropertyName("Passengers_Missions_Bulk")]
        public int MissionsBulk { get; init; }

        [JsonPropertyName("Passengers_Missions_VIP")]
        public int MissionsVIP { get; init; }

        [JsonPropertyName("Passnegers_Missions_Delivered")]
        public int MissionsDelivered { get; init; }

        [JsonPropertyName("Passengers_Missions_Ejected")]
        public int MissionsEjected { get; init; }
    }
}
