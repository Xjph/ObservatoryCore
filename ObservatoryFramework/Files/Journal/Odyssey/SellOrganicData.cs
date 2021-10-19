using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class SellOrganicData : JournalBase
    {
        public ulong MarketID { get; init; }
        public ImmutableList<BioData> BioData { get; init; }
    }
}
