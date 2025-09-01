using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierDepositFuel : JournalBase
    {
        public ulong CarrierID { get; init; }
        public CarrierType CarrierType { get; init; }
        public int Amount { get; init; }
        public int Total { get; init; }
    }
}
