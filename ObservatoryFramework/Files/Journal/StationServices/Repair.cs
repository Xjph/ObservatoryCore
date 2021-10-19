using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Repair : JournalBase
    {
        public string Item { get; init; }
        public int Cost { get; init; }
        public ImmutableList<string> Items { get; init; }
    }
}
