using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class SellMicroResources : JournalBase
    {
        public ImmutableList<MicroResource> MicroResources { get; init; }
        public int Price { get; init; }
        public ulong MarketID { get; init; }
    }
}
