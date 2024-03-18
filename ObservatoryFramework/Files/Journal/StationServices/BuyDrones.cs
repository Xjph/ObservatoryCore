namespace Observatory.Framework.Files.Journal
{
    public class BuyDrones : JournalBase
    {
        public string Type { get; init; }
        public int Count { get; init; }
        public uint BuyPrice { get; init; }
        public uint SellPrice { get; init; }
        public int TotalCost { get; init; }
    }
}
