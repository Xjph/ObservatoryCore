using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierCancelDecommission : JournalBase
    {
        public ulong CarrierID { get; init; }
        public CarrierType CarrierType { get; init; }
    }
}
