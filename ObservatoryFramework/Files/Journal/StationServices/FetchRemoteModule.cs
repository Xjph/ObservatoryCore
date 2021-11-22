namespace Observatory.Framework.Files.Journal
{
    public class FetchRemoteModule : JournalBase
    {
        public ulong ShipID { get; init; }
        public int StorageSlot { get; init; }
        public string StoredItem { get; init; }
        public string StoredItem_Localised { get; init; }
        public ulong ServerId { get; init; }
        public long TransferCost { get; init; }
        public string Ship { get; init; }
        public long TransferTime { get; init; }
    }
}
