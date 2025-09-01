using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierNameChange
    {
        public ulong CarrierID { get; init; }
        public CarrierType CarrierType { get; init; }
        public string Name { get; init; }
        public string Callsign { get; init; }
    }
}
