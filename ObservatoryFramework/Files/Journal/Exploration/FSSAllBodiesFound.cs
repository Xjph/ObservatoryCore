namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated when all bodies within a system have been scanned.
    /// </summary>
    public class FSSAllBodiesFound : JournalBase
    {
        /// <summary>
        /// Name of the system.
        /// </summary>
        public string SystemName { get; init; }
        /// <summary>
        /// Unique ID of the system.
        /// </summary>
        public ulong SystemAddress { get; init; }
        /// <summary>
        /// Count of all scanned bodies in system.
        /// </summary>
        public int Count { get; init; }
    }
}
