namespace Observatory.Framework.Files.Journal
{
    public class MarketSell : JournalBase
    {
        public long MarketID { get; init; }
        public string Type { get; init; }
        public string Type_Localised { get; init; }
        public int Count { get; init; }
        public int SellPrice { get; init; }
        public long TotalSale { get; init; }
        public int AvgPricePaid { get; init; }
        public bool IllegalGoods { get; init; }
        public bool StolenGoods { get; init; }
        public bool BlackMarket { get; init; }
    }
}
