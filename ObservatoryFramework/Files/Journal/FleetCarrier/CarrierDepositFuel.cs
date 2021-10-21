namespace Observatory.Framework.Files.Journal
{
    public class CarrierDepositFuel : JournalBase
    {
        public long CarrierID { get; init; }
        public int Amount { get; init; }
        public int Total { get; init; }
    }
}
