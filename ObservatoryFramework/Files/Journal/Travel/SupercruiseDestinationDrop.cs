namespace Observatory.Framework.Files.Journal
{
    public class SupercruiseDestinationDrop : JournalBase
    {
        public string Type { get; init; }
        public int Threat { get; init; }
        public ulong MarketID { get; init; }
    }
}
