using System.Collections.Immutable;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class SuitModule
    {
        public string SlotName { get; init; }
        public string ModuleName { get; init; }
        public ulong SuitModuleID { get; init; }
        public int Class { get; init; }
        public ImmutableList<string> WeaponMods { get; init; }
    }
}
