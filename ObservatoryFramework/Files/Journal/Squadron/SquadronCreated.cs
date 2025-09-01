namespace Observatory.Framework.Files.Journal
{
    public class SquadronCreated : JournalBase
    {
        public ulong SquadronID { get; init; }
        public string SquadronName { get; init; }
    }
}
