using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class CQC
    {
        [JsonPropertyName("CQC_Credits_Earned")]
        public long CreditsEarned { get; init; }

        [JsonPropertyName("CQC_Time_Played")]
        public long TimePlayed { get; init; }

        [JsonPropertyName("CQC_KD")]
        public double KillDeathRatio { get; init; }

        [JsonPropertyName("CQC_Kills")]
        public int Kills { get; init; }

        [JsonPropertyName("CQC_WL")]
        public double WinLossRatio { get; init; }
    }
}
