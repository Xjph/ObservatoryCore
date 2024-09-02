using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Trading
    {
        [JsonPropertyName("Markets_Traded_With")]
        public int MarketsTradedWith { get; init; }

        [JsonPropertyName("Market_Profits")]
        public long MarketProfits { get; init; }

        [JsonPropertyName("Resources_Traded")]
        public int ResourcesTraded { get; init; }

        [JsonPropertyName("Average_Profit")]
        public decimal AverageProfit { get; init; }

        [JsonPropertyName("Highest_Single_Transaction")]
        public long HighestSingleTransaction { get; init; }

        [JsonPropertyName("Assets_Sold")]
        public int AssetsSold { get; init; }

        [JsonPropertyName("Data_Sold")]
        public int DataSold { get; init; }

        [JsonPropertyName("Goods_Sold")]
        public int GoodsSold { get; init; }
    }
}
