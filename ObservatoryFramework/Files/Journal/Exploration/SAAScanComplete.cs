using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated when a body surface scan is completed.
    /// </summary>
    public class SAAScanComplete : JournalBase
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
        /// ID of the scanned body within the system.
        /// </summary>
        public int BodyID { get; init; }
        /// <summary>
        /// This property is indicated with strikethrough in Frontier's documentation and is likely unused.
        /// </summary>
        public ImmutableList<string> Discoverers { get; init; }
        /// <summary>
        /// This property is indicated with strikethrough in Frontier's documentation and is likely unused.
        /// </summary>
        public ImmutableList<string> Mappers { get; init; }
        /// <summary>
        /// Number of probes fired to complete the surface scan.
        /// </summary>
        public int ProbesUsed { get; init; }
        /// <summary>
        /// Maximum number of probes which can be used to get efficiency bonus.
        /// </summary>
        public int EfficiencyTarget { get; init; }
    }
}
