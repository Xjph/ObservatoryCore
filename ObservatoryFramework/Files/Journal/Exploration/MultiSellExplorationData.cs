using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class MultiSellExplorationData : JournalBase
    {
        public ImmutableList<Discovered> Discovered { get; init; }
        public long BaseValue { get; init; }
        public long Bonus { get; init; }
        public long TotalEarnings { get; init; }

    }
}
