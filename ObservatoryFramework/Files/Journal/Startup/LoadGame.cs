using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class LoadGame : JournalBase
    {
        public string Commander { get; init; }
        public string FID { get; init; }
        public bool Horizons { get; init; }
        public bool Odyssey { get; init; }
        public string Ship { get; init; }
        public string Ship_Localised { get; init; }
        public ulong ShipID { get; init; }
        public bool StartLanded { get; init; }
        public bool StartDead { get; init; }
        public string GameMode { get; init; }
        public string Group { get; init; }
        public long Credits { get; init; }
        public long Loan { get; init; }
        public string ShipName { get; init; }
        public string ShipIdent { get; init; }
        public double FuelLevel { get; init; }
        public double FuelCapacity { get; init; }
        [JsonPropertyName("language")]
        public string Language { get; init; }
        [JsonPropertyName("gameversion")]
        public string GameVersion { get; init; }
        [JsonPropertyName("build")]
        public string Build { get; init; }
    }
}
