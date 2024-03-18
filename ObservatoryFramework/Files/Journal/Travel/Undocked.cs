namespace Observatory.Framework.Files.Journal
{
    public class Undocked : JournalBase
    {
        /// <summary>
        /// Name of the station at which this event occurred.
        /// </summary>
        public string StationName { get; init; }
        public string StationType { get; init; }
        public ulong MarketID { get; init; }
        public bool Taxi { get; init; }
        public bool Multicrew { get; init; }
    }
}
