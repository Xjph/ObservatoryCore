namespace Observatory.Framework.Files.Journal
{
    public class CarrierDepositFuel : JournalBase
    {
        public ulong CarrierID { get; init; }
        public int Amount { get; init; }
        public int Total { get; init; }
    }
}
