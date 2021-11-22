namespace Observatory.Framework.Files.Journal
{
    public class MissionFailed : JournalBase
    {
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public ulong MissionID { get; init; }
        public long Fine { get; init; }
    }
}
