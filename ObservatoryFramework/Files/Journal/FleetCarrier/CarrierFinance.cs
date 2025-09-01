using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierFinance : JournalBase
    {
        public ulong CarrierID { get; init; }
        public CarrierType CarrierType { get; init; }
        public int TaxRate { get; init; }
        public long CarrierBalance { get; init; }
        public long ReserveBalance { get; init; }
        public long AvailableBalance { get; init; }
        public int ReservePercent { get; init; }
    }
}
