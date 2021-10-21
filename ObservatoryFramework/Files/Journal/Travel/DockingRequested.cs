using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class DockingRequested : JournalBase
    {
        public string StationName { get; init; }
        public string StationType { get; init; }
        public long MarketID { get; init; }
        public LandingPads LandingPads { get; init; }
    }
}
