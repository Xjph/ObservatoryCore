using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Combat
    {
        [JsonPropertyName("Bounties_Claimed")]
        public int BountiesClaimed { get; init; }

        [JsonPropertyName("Bounty_Hunting_Profit")]
        public decimal BountyHuntingProfit { get; init; }

        [JsonPropertyName("Combat_Bonds")]
        public int CombatBonds { get; init; }

        [JsonPropertyName("Combat_Bond_Profits")]
        public decimal CombatBondProfits { get; init; }

        public int Assassinations { get; init; }

        [JsonPropertyName("Assassination_Profits")]
        public decimal AssassinationProfits { get; init; }

        [JsonPropertyName("Highest_Single_Reward")]
        public decimal HighestSingleReward { get; init; }

        [JsonPropertyName("Skimmers_Killed")]
        public int SkimmersKilled { get; init; }
    }
}
