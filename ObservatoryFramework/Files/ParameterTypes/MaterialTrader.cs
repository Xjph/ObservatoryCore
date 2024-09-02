using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class MaterialTrader
    {
        [JsonPropertyName("Assets_Traded_In")]
        public int AssetsTradedIn { get; init; }

        [JsonPropertyName("Assets_Traded_Out")]
        public int AssetsTradedOut { get; init; }

        [JsonPropertyName("Trades_Completed")]
        public int TradesCompleted { get; init; }

        [JsonPropertyName("Materials_Traded")]
        public int MaterialsTraded { get; init; }

        [JsonPropertyName("Encoded_Materials_Traded")]
        public int EncodedMaterialsTraded { get; init; }

        [JsonPropertyName("Raw_Materials_Traded")]
        public int RawMaterialsTraded { get; init; }

        public int DataMaterialsTraded
        {
            get
            {
                return MaterialsTraded - EncodedMaterialsTraded - RawMaterialsTraded;
            }
        }

        [JsonPropertyName("Grade_1_Materials_Traded")]
        public int Grade1MaterialsTraded { get; init; }

        [JsonPropertyName("Grade_2_Materials_Traded")]
        public int Grade2MaterialsTraded { get; init; }

        [JsonPropertyName("Grade_3_Materials_Traded")]
        public int Grade3MaterialsTraded { get; init; }

        [JsonPropertyName("Grade_4_Materials_Traded")]
        public int Grade4MaterialsTraded { get; init; }

        [JsonPropertyName("Grade_5_Materials_Traded")]
        public int Grade5MaterialsTraded { get; init; }
    }
}
