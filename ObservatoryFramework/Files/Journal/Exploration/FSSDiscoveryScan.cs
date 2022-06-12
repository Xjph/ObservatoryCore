namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated when using the discovery scanner (honk) to initially scan system.
    /// </summary>
    public class FSSDiscoveryScan : JournalBase
    {
        /// <summary>
        /// Name of the current system.
        /// </summary>
        public string SystemName { get; init; }
        /// <summary>
        /// Unique ID of the current system.
        /// </summary>
        public ulong SystemAddress { get; init; }
        /// <summary>
        /// Percentage of current system already scanned.
        /// </summary>
        public float Progress { get; init; }
        /// <summary>
        /// Number of scannable bodies in system.
        /// </summary>
        public int BodyCount { get; init; }
        /// <summary>
        /// Number of scannable non-body locations in system.
        /// </summary>
        public int NonBodyCount { get; init; }
    }
}
