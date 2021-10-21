namespace Observatory.Framework.Files.Journal
{
    public class PayBounties : JournalBase
    {
        public long Amount { get; init; }
        public float BrokerPercentage { get; init; }
        public bool AllFines { get; init; }
        public string Faction { get; init; }
        public string Faction_Localised { get; init; }
        public ulong ShipID { get; init; }
    }
}
