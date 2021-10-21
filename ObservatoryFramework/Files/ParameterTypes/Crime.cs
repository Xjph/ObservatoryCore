using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Crime
    {
        public int Notoriety { get; init; }

        public long Fines { get; init; }

        [JsonPropertyName("Total_Fines")]
        public long TotalFines { get; init; }

        [JsonPropertyName("Bounties_Received")]
        public int BountiesReceived { get; init; }

        [JsonPropertyName("Total_Bounties")]
        public decimal TotalBounties { get; init; }

        [JsonPropertyName("Highest_Bounty")]
        public decimal HighestBounty { get; init; }
    }
}
