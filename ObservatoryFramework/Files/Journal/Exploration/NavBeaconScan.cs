namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated when scanned a populated system's navigation beacon.
    /// </summary>
    public class NavBeaconScan : JournalBase
    {
        /// <summary>
        /// Number of bodies in system.
        /// </summary>
        public int NumBodies { get; init; }
        /// <summary>
        /// Unique ID of system.
        /// </summary>
        public ulong SystemAddress { get; init; }
    }
}
