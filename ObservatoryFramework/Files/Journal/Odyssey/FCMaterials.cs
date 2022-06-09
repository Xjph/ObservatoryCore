namespace Observatory.Framework.Files.Journal
{
    public class FCMaterials : JournalBase
    {
        public ulong MarketID { get; init; }
        public string CarrierName { get; init; }
        public string CarrierID { get; init; }
    }
}