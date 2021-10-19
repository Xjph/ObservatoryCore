namespace Observatory.Framework.Files.Journal
{
    public class LeaveBody : JournalBase
    {
        public string StarSystem { get; init; }
        public ulong SystemAddress { get; init; }
        public string Body { get; init; }
        public int BodyID { get; init; }
    }
}
