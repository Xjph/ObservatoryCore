namespace Observatory.Framework.Files.Journal
{
    public class DiscoveryScan : JournalBase
    {
        public ulong SystemAddress { get; init; }
        public int Bodies { get; init; }
    }
}
