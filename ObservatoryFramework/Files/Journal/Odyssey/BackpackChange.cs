using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class BackpackChange : JournalBase
    {
        public ImmutableList<BackpackItemChange> Added { get; init; }
        public ImmutableList<BackpackItemChange> Removed { get; init; }
    }
}
