using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class TradeMicroResources : JournalBase
    {
        public ImmutableList<MicroResource> Offered { get; init; }
        public string Received { get; init; }
        public MicroCategory Category { get; init; }
        public int Count { get; init; }
        public ulong MarketID { get; init; }
    }
}
