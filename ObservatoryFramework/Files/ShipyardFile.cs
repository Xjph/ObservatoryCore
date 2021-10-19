using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    public class ShipyardFile : Journal.JournalBase
    {
        public long MarketID { get; init; }
        public string StationName { get; init; }
        public string StarSystem { get; init; }
        public bool Horizons { get; init; }
        public bool AllowCobraMkIV { get; init; }
        public ImmutableList<ShipyardPrice> PriceList { get; init; }
    }
}
