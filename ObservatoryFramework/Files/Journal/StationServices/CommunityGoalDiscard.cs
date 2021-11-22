namespace Observatory.Framework.Files.Journal
{
    public class CommunityGoalDiscard : JournalBase
    {
        public ulong CGID { get; init; }
        public string Name { get; init; }
        public string System { get; init; }
    }
}
