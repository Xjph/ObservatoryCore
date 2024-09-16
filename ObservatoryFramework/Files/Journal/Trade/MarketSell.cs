namespace Observatory.Framework.Files.Journal
{
    public class MarketSell : JournalBase
    {
        public ulong MarketID { get; init; }
        public string Type { get; init; }
        public string Type_Localised { get; init; }
        public int Count { get; init; }
        public uint SellPrice { get; init; }
        public long TotalSale { get; init; }
        public uint AvgPricePaid { get; init; }
        public bool IllegalGoods { get; init; }
        public bool StolenGoods { get; init; }
        public bool BlackMarket { get; init; }
    }
}
