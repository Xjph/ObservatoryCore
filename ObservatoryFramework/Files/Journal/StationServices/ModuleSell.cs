namespace Observatory.Framework.Files.Journal
{
    public class ModuleSell : JournalBase
    {
        public ulong MarketID { get; init; }
        public string Slot { get; init; }
        public string SellItem { get; init; }
        public string SellItem_Localised { get; init; }
        public int SellPrice { get; init; }
        public string Ship { get; init; }
        public ulong ShipID { get; init; }
    }
}
