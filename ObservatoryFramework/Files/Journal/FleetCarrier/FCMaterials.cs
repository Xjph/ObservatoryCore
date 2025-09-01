namespace Observatory.Framework.Files.Journal
{
    public class FCMaterials : JournalBase
    {
        public ulong MarketID { get; init; }
        public string CarrierName { get; init; }
        public ulong CarrierID { get; init; }
    }
}
