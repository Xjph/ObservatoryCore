using System.Collections.Immutable;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CreateSuitLoadout : DeleteSuitLoadout
    {
        public ImmutableList<SuitModule> Modules { get; init; }
        public ImmutableList<string> SuitMods { get; init; }
    }
}
