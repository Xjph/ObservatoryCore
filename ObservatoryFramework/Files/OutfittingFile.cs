using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    public class OutfittingFile : Journal.JournalBase
    {
        public long MarketID { get; init; }
        public string StationName { get; init; }
        public string StarSystem { get; init; }
        public bool Horizons { get; init; }
        public ImmutableList<OutfittingModule> Items { get; init; }
    }
}
