namespace Observatory.Framework.Files.Journal
{
    public class Undocked : JournalBase
    {
        public string StationName { get; init; }
        public string StationType { get; init; }
        public ulong MarketID { get; init; }
        public bool Taxi { get; init; }
        public bool Multicrew { get; init; }
    }
}
