namespace Observatory.Framework.Files.Journal
{
    public class EjectCargo : JournalBase
    {
        public string Type { get; init; }
        public string Type_Localised { get; init; }
        public int Count { get; init; }
        public int MissionID { get; init; }
        public bool Abandoned { get; init; }
        public string PowerplayOrigin { get; init; }
    }
}
