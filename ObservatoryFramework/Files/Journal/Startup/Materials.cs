using System.Collections.Immutable;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class Materials : JournalBase
    {
        public ImmutableList<Material> Raw { get; init; }
        public ImmutableList<Material> Manufactured { get; init; }
        public ImmutableList<Material> Encoded { get; init; }
    }
}
