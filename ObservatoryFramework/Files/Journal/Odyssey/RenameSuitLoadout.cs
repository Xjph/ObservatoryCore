namespace Observatory.Framework.Files.Journal
{
    public class RenameSuitLoadout : JournalBase
    {
        public ulong SuitID { get; init; }
        public string SuitName { get; init; }
        public ulong LoadoutID { get; init; }
        public string LoadoutName { get; init; }
    }
}
