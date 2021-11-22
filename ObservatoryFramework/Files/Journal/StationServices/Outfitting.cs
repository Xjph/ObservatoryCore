namespace Observatory.Framework.Files.Journal
{
    public class Outfitting : JournalBase
    {
        public ulong MarketID { get; init; }
        public string StationName { get; init; }
        public string StarSystem { get; init; }
    }
}
