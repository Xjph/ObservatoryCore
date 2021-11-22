using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class TechnologyBroker : JournalBase
    {
        public string BrokerType { get; init; }
        public ulong MarketID { get; init; }
        public ImmutableList<ItemName> ItemsUnlocked { get; init; }
        public ImmutableList<CommodityReward> Commodities { get; init; }
        public ImmutableList<MaterialReward> Materials { get; init; }
    }
}
