using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class RebootRepair : JournalBase
    {
        public ImmutableList<string> Modules { get; init; }
    }
}
