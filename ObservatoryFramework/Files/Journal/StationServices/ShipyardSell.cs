namespace Observatory.Framework.Files.Journal
{
    public class ShipyardSell : JournalBase
    {
        public long MarketID { get; init; }
        public string ShipType { get; init; }
        public int SellShipID { get; init; }
        public long ShipPrice { get; init; }
        public string System { get; init; }
    }
}
