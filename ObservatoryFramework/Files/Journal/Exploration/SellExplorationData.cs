using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class SellExplorationData : JournalBase
    {
        public ImmutableList<string> Systems { get; init; }
        public ImmutableList<string> Discovered { get; init; }
        public long BaseValue { get; init; }
        public long Bonus { get; init; }
        public long TotalEarnings { get; init; }
    }
}
