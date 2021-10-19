using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Cargo : JournalBase
    {
        public string Vessel { get; init; }
        public int Count { get; init; }
        public ImmutableList<Cargo> Inventory { get; init; }
    }
}
