namespace Observatory.Framework.Files.Journal.StationServices
{
    public class ModuleBuyAndStore : JournalBase
    {
        public string BuyItem { get; init; }
        public string BuyItem_Localised { get; init; }
        public ulong MarketID { get; init; }
        public uint BuyPrice { get; init; }
        public uint BuyMercCoinsPrice { get; init; }
        public string Ship { get; init; }
        public ulong ShipID { get; init; }
    }
}
