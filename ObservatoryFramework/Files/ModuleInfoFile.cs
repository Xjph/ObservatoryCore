using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    /// <summary>
    /// Information about current player ship equipped modules.
    /// </summary>
    public class ModuleInfoFile : Journal.JournalBase
    {
        /// <summary>
        /// List of all equipped modules.
        /// </summary>
        public ImmutableList<Module> Modules { get; init; }
    }
}
