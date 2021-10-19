namespace Observatory.Framework.Files.Journal
{
    public class FSDTarget : JournalBase
    {
        public string Name { get; init; }
        public ulong SystemAddress { get; init; }
        public string StarClass { get; init; }
        public int RemainingJumpsInRoute { get; init; }
    }
}
