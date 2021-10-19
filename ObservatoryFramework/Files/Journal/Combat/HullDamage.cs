namespace Observatory.Framework.Files.Journal
{
    public class HullDamage : JournalBase
    {
        public float Health { get; init; }
        public bool PlayerPilot { get; init; }
        public bool Fighter { get; init; }
    }
}
