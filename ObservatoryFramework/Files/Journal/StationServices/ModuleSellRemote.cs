namespace Observatory.Framework.Files.Journal
{
    public class ModuleSellRemote : JournalBase
    {
        public int StorageSlot { get; init; }
        public string SellItem { get; init; }
        public string SellItem_Localised { get; init; }
        public ulong ServerId { get; init; }
        public int SellPrice { get; init; }
        public string Ship { get; init; }
        public ulong ShipID { get; init; }
    }
}
