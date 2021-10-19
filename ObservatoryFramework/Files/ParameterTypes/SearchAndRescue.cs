using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class SearchAndRescue
    {
        [JsonPropertyName("SearchRescue_Traded")]
        public int Traded { get; init; }

        [JsonPropertyName("SearchRescue_Profit")]
        public long Profit { get; init; }

        [JsonPropertyName("SearchRescue_Count")]
        public int Count { get; init; }
    }
}
