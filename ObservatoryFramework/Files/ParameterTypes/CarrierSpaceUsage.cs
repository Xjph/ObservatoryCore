namespace Observatory.Framework.Files.ParameterTypes
{
    public class CarrierSpaceUsage
    {
        public int TotalCapacity { get; init; }
        public int Crew { get; init; }
        public int Cargo { get; init; }
        public int CargoSpaceReserved { get; init; }
        public int ShipPacks { get; init; }
        public int ModulePacks { get; init; }
        public int FreeSpace { get; init; }
    }
}
