using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Multicrew
    {
        [JsonPropertyName("Multicrew_Time_Total")]
        public long TimeTotal { get; init; }

        [JsonPropertyName("Multicrew_Gunner_Time_Total")]
        public long GunnerTimeTotal { get; init; }

        [JsonPropertyName("Multicrew_Fighter_Time_Total")]
        public long FighterTimeTotal { get; init; }

        [JsonPropertyName("Multicrew_Credits_Total")]
        public long CreditsTotal { get; init; }

        [JsonPropertyName("Multicrew_Fines_Total")]
        public long FinesTotal { get; init; }
    }
}
