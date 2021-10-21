using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    public class ModulesInfo : Journal.JournalBase
    {
        public ImmutableList<Module> Modules { get; init; }
    }
}
