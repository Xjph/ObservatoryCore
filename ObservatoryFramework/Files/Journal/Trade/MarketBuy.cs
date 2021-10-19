namespace Observatory.Framework.Files.Journal
{
    public class MarketBuy : JournalBase
    {
        public long MarketID { get; init; }
        public string Type { get; init; }
        public string Type_Localised { get; init; }
        public int Count { get; init; }
        public int BuyPrice { get; init; } 
        public long TotalCost { get; init; }
    }
}
