namespace Observatory.Framework.Files.Journal
{
    public class SellDrones : JournalBase
    {
        public string Type { get; init; }
        public int Count { get; init; }
        public uint SellPrice { get; init; }
        public int TotalSale { get; init; }
    }
}
