namespace Observatory.Framework.Files.Journal
{
    public class Shipyard : JournalBase
    {
        public long MarketID { get; init; }
        public string StationName { get; init; }
        public string StarSystem { get; init; }
    }
}
