namespace Observatory.Framework.Files.Journal
{
    public class DatalinkVoucher : JournalBase
    {
        public int Reward { get; init; }
        public string VictimFaction { get; init; }
        public string PayeeFaction { get; init; }
    }
}
