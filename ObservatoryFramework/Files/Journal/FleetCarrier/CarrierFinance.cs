namespace Observatory.Framework.Files.Journal
{
    public class CarrierFinance : JournalBase
    {
        public long CarrierID { get; init; }
        public int TaxRate { get; init; }
        public long CarrierBalance { get; init; }
        public long ReserveBalance { get; init; }
        public long AvailableBalance { get; init; }
        public int ReservePercent { get; init; }
    }
}
