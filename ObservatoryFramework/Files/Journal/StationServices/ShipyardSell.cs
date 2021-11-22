namespace Observatory.Framework.Files.Journal
{
    public class ShipyardSell : JournalBase
    {
        public ulong MarketID { get; init; }
        public string ShipType { get; init; }
        public ulong SellShipID { get; init; }
        public long ShipPrice { get; init; }
        public string System { get; init; }
    }
}
