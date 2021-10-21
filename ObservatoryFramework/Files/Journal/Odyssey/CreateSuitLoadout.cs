using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class CreateSuitLoadout : DeleteSuitLoadout
    {
        public ImmutableList<SuitModule> Modules { get; init; }
        public ImmutableList<string> SuitMods { get; init; }
    }
}
