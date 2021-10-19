using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    public class MarketFile : Journal.JournalBase
    {
        public long MarketID { get; init; }
        public string StationName { get; init; }
        public string StationType { get; init; }
        public string StarSystem { get; init; }
        public ImmutableList<MarketItem> Items { get; init; }
    }
}
