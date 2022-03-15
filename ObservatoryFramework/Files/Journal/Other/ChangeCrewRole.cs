namespace Observatory.Framework.Files.Journal
{
    public class ChangeCrewRole : JournalBase
    {
        public string Role { get; init; }
        public bool Telepresence { get; init; }
    }
}
