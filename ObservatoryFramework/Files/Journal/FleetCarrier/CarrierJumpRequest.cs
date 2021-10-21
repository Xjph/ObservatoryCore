namespace Observatory.Framework.Files.Journal
{
    public class CarrierJumpRequest : JournalBase
    {
        public string Body { get; init; }
        public int BodyID { get; init; }
        public ulong SystemAddress { get; init; }
        public long CarrierID { get; init; }
        public string SystemName { get; init; }
        public ulong SystemID { get; init; }
    }
}
