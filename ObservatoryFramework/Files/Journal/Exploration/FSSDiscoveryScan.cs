namespace Observatory.Framework.Files.Journal
{
    public class FSSDiscoveryScan : JournalBase
    {
        public string SystemName { get; init; }
        public ulong SystemAddress { get; init; }
        public float Progress { get; init; }
        public int BodyCount { get; init; }
        public int NonBodyCount { get; init; }
    }
}
