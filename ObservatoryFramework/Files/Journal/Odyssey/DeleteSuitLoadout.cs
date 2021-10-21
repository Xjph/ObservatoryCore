namespace Observatory.Framework.Files.Journal
{
    public class DeleteSuitLoadout : JournalBase
    {
        public ulong SuitID { get; init; }
        public string SuitName { get; init; }
        public string SuitName_Localised { get; init; }
        public ulong LoadoutID { get; init; }
        public string LoadoutName { get; init; }
    }
}
