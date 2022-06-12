using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event written when the surface scan finds signals on the body.
    /// </summary>
    public class SAASignalsFound : JournalBase
    {
        /// <summary>
        /// Unique ID of current system.
        /// </summary>
        public ulong SystemAddress { get; init; }
        /// <summary>
        /// Name of the scanned body.
        /// </summary>
        public string BodyName { get; init; }
        /// <summary>
        /// ID of the body within the system.
        /// </summary>
        public int BodyID { get; init; }
        /// <summary>
        /// List of signals found.
        /// </summary>
        public ImmutableList<Signal> Signals { get; init; }
    }
}
