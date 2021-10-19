namespace Observatory.Framework.Files.Journal
{
    public class CollectCargo : JournalBase
    {
        public string Type { get; init; }
        public string Type_Localised { get; init; }
        public bool Stolen { get; init; }
        public int MissionID { get; init; }
    }
}
