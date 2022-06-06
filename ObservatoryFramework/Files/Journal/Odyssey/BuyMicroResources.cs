using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class BuyMicroResources : JournalBase
    {
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MicroCategory Category { get; init; }
        public int Count { get; init; }
        public int Price { get; init; }
        public ulong MarketID { get; init; }
        public int TotalCount { get; init; }
        public ImmutableList<MicroResource> MicroResources { get; init; }
    }
}
