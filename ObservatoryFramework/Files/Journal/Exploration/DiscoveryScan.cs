namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated when using the discovery scanner (honk) to initially scan system. No longer used in live game client, will be found only in historical data.
    /// </summary>
    public class DiscoveryScan : JournalBase
    {
        /// <summary>
        /// Unique ID of system.
        /// </summary>
        public ulong SystemAddress { get; init; }
        /// <summary>
        /// Number of bodies in system.
        /// </summary>
        public int Bodies { get; init; }
    }
}
