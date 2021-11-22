namespace Observatory.Framework.Files.Journal
{
    public class SellShipOnRebuy : JournalBase
    {
        public string ShipType { get; init; }
        public string System { get; init; }
        public ulong SellShipId { get; init; }
        public long ShipPrice { get; init; }
    }
}
