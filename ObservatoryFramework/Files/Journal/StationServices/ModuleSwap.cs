namespace Observatory.Framework.Files.Journal
{
    public class ModuleSwap : JournalBase
    {
        public ulong MarketID { get; init; }
        public string FromSlot { get; init; }
        public string ToSlot { get; init; }
        public string FromItem { get; init; }
        public string FromItem_Localised { get; init; }
        public string ToItem { get; init; }
        public string ToItem_Localised { get; init; }
        public string Ship { get; init; }
        public ulong ShipID { get; init; }
    }
}
