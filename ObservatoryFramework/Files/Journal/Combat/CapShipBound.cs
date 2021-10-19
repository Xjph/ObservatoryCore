namespace Observatory.Framework.Files.Journal
{
    public class CapShipBound : JournalBase
    {
        public long Reward { get; init; }
        public string AwardingFaction { get; init; }
        public string VictimFaction { get; init; }
    }
}
