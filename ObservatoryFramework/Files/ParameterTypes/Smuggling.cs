using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Smuggling
    {
        [JsonPropertyName("Black_Markets_Traded_With")]
        public int BlackMarketsTradedWith { get; init; }

        [JsonPropertyName("Black_Markets_Profits")]
        public long BlackMarketsProfits { get; init; }

        [JsonPropertyName("Resources_Smuggled")]
        public int ResourcesSmuggled { get; init; }

        [JsonPropertyName("Average_Profit")]
        public decimal AverageProfit { get; init; }

        [JsonPropertyName("Highest_Single_Transaction")]
        public long HighestSingleTransaction { get; init; }
    }
}
