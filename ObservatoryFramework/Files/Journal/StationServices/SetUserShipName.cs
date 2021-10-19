namespace Observatory.Framework.Files.Journal
{
    public class SetUserShipName : JournalBase
    {
        public string Ship { get; init; }
        public ulong ShipID { get; init; }
        public string UserShipName { get; init; }
        public string UserShipId { get; init; }
    }
}
