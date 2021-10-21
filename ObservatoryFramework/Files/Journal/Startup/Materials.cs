using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Materials : JournalBase
    {
        public ImmutableList<Material> Raw { get; init; }
        public ImmutableList<Material> Manufactured { get; init; }
        public ImmutableList<Material> Encoded { get; init; }

    }
}
