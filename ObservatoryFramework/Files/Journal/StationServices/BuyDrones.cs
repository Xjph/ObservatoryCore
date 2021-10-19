namespace Observatory.Framework.Files.Journal
{
    public class BuyDrones : JournalBase
    {
        public string Type { get; init; }
        public int Count { get; init; }
        public int BuyPrice { get; init; }
        public int SellPrice { get; init; }
        public int TotalCost { get; init; }
    }
}
