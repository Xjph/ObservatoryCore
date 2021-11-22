namespace Observatory.Framework.Files.Journal
{
    public class SearchAndRescue : JournalBase
    {
        public ulong MarketID { get; init; }
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public int Count { get; init; }
        public int Reward { get; init; }
    }
}
