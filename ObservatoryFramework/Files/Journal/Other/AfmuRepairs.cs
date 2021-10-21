namespace Observatory.Framework.Files.Journal
{
    public class AfmuRepairs : JournalBase
    {
        public string Module { get; init; }
        public string Module_Localised { get; init; }
        public bool FullyRepaired { get; init; }
        public float Health { get; init; }
    }
}
