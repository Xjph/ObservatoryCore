namespace Observatory.Framework.Files.Journal.Other
{
    public class LaunchVessel : JournalBase
    {
        public string Loadout { get; init; }
        public ulong ID { get; init; }
        public bool PlayerControlled { get; init; }
        public string VesselType { get; init; }
        public string VesselType_Localised { get; init; }
    }
}
