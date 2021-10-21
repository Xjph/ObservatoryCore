using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class StoredModules : JournalBase
    {
        public string StarSystem { get; init; }
        public string StationName { get; init; }
        public long MarketID { get; init; }
        public ImmutableList<StoredItem> Items { get; init; }
    }
}
