using System.Collections.Immutable;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class MassModuleStore : JournalBase
    {
        public ulong MarketID { get; init; }
        public string Ship { get; init; }
        public ulong ShipID { get; init; }
        public ImmutableList<Item> Items { get; init; }
    }
}
