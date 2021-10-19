namespace Observatory.Framework.Files.Journal
{
    public class ShipyardBuy : JournalBase
    {
        public long MarketID { get; init; }
        public string ShipType { get; init; }
        public string ShipType_Localised { get; init; }
        public long ShipPrice { get; init; }
        public string StoreOldShip { get; init; }
        public int StoreShipID { get; init; }
        public string SellOldShip { get; init; }
        public int SellShipID { get; init; }
        public long SellPrice { get; init; }
    }
}
