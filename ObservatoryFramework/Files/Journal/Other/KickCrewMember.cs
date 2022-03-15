namespace Observatory.Framework.Files.Journal
{
    public class KickCrewMember : JournalBase
    {
        public string Crew { get; init; }
        public bool OnCrime { get; init; }
        public bool Telepresence { get; init; }
    }
}
