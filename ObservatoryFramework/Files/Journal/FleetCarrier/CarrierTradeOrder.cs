namespace Observatory.Framework.Files.Journal
{
    public class CarrierTradeOrder : JournalBase
    {
        public ulong CarrierID { get; init; }
        public bool BlackMarket { get; init; }
        public string Commodity { get; init; }
        public string Commodity_Localised { get; init; }
        public int PurchaseOrder { get; init; }
        public int SaleOrder { get; init; }
        public bool CancelTrade { get; init; }
        public int Price { get; init; }
    }
}
