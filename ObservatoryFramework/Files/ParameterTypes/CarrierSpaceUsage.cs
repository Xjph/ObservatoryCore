namespace Observatory.Framework.Files.ParameterTypes
{
    public class CarrierSpaceUsage
    {
        public long TotalCapacity { get; init; }
        public long Crew { get; init; }
        public long Cargo { get; init; }
        public long CargoSpaceReserved { get; init; }
        public long ShipPacks { get; init; }
        public long ModulePacks { get; init; }
        public long FreeSpace { get; init; }
    }
}
