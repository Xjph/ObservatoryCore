namespace Observatory.Framework.Files.Journal
{
    public class CarrierBankTransfer : JournalBase
    {
        public long CarrierID { get; init; }
        public long Deposit { get; init; }
        public long Withdraw { get; init; }
        public long PlayerBalance { get; init; }
        public long CarrierBalance { get; init; }
    }
}
