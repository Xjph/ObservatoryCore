namespace Observatory.Framework.Files.Journal
{
    public class FSSAllBodiesFound : JournalBase
    {
        public string SystemName { get; init; }
        public ulong SystemAddress { get; init; }
        public int Count { get; init; }
    }
}
