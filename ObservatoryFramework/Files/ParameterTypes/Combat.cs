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
        [JsonPropertyName("ConflictZone_High")]
        public int ConflictZoneHigh { get; init; }
        [JsonPropertyName("ConflictZone_High_Wins")]
        public int ConflictZoneHighWins { get; init; }
        [JsonPropertyName("ConflictZone_Low")]
        public int ConflictZoneLow { get; init; }
        [JsonPropertyName("ConflictZone_Low_Wins")]
        public int ConflictZoneLowWins { get; init; }
        [JsonPropertyName("ConflictZone_Medium")]
        public int ConflictZoneMedium { get; init; }
        [JsonPropertyName("ConflictZone_Medium_Wins")]
        public int ConflictZoneMediumWins { get; init; }
        [JsonPropertyName("ConflictZone_Total")]
        public int ConflictZoneTotal { get; init; }
        [JsonPropertyName("ConflictZone_Total_Wins")]
        public int ConflictZoneTotalWins { get; init; }
        [JsonPropertyName("OnFoot_Combat_Bonds")]
        public int OnFootCombatBonds { get; init; }
        [JsonPropertyName("OnFoot_Combat_Bonds_Profits")]
        public long OnFootCombatBondsProfits { get; init; }
        [JsonPropertyName("OnFoot_Scavs_Killed")]
        public int OnFootScavsKilled { get; init; }
        [JsonPropertyName("OnFoot_Ships_Destroyed")]
        public int OnFootShipsDestroyed { get; init; }
        [JsonPropertyName("OnFoot_Skimmers_Killed")]
        public int OnFootSkimmersKilled { get; init; }
        [JsonPropertyName("OnFoot_Vehicles_Destroyed")]
        public int OnFootVehiclesDestroyed { get; init; }
        [JsonPropertyName("Settlement_Conquered")]
        public int SettlementConquered {  get; init; }
        [JsonPropertyName("Settlement_Defended")]
        public int SettlementDefended { get; init; }
    }
}
