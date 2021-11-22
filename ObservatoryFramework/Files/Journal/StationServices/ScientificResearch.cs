namespace Observatory.Framework.Files.Journal
{
    public class ScientificResearch : JournalBase
    {
        public ulong MarketID { get; init; }
        public string Name { get; init; }
        public string Category { get; init; }
        public int Count { get; init; }
    }
}
