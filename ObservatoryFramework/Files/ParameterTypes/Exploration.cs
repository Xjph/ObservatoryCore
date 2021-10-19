using System;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Exploration
    {
        [JsonPropertyName("Systems_Visited")]
        public long SystemsVisited { get; init; }

        [JsonPropertyName("Fuel_Scooped"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public int FuelScooped { get; init; }

        [JsonPropertyName("Fuel_Purchased"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public int FuelPurchased { get; init; }

        [JsonPropertyName("Exploration_Profits")]
        public long ExplorationProfits { get; init; }

        [JsonPropertyName("Planets_Scanned_To_Level_2")]
        public long PlanetsScannedToLevel2 { get; init; }

        [JsonPropertyName("Planets_Scanned_To_Level_3")]
        public long PlanetsScannedToLevel3 { get; init; }

        [JsonPropertyName("Highest_Payout")]
        public long HighestPayout { get; init; }

        [JsonPropertyName("Total_Hyperspace_Distance")]
        public long TotalHyperspaceDistance { get; init; }

        [JsonPropertyName("Total_Hyperspace_Jumps")]
        public long TotalHyperspaceJumps { get; init; }

        [JsonPropertyName("Greatest_Distance_From_Start")]
        public double GreatestDistanceFromStart { get; init; }

        [JsonPropertyName("Time_Played")]
        public long TimePlayed { get; init; }

        [JsonPropertyName("Efficient_Scans")]
        public int EfficientScans { get; init; }
    }
}
