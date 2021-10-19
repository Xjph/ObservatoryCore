namespace Observatory.Framework.Files.Journal
{
    public class RepairDrone : JournalBase
    {
        public float HullRepaired { get; init; }
        public float CockpitRepaired { get; init; }
        public float CorrosionRepaired { get; init; }
    }
}
