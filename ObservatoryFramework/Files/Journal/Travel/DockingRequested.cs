using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class DockingRequested : JournalBase
    {
        /// <summary>
        /// Name of the station at which this event occurred.
        /// </summary>
        public string StationName { get; init; }
        public string StationType { get; init; }
        public ulong MarketID { get; init; }
        public LandingPads LandingPads { get; init; }
    }
}
