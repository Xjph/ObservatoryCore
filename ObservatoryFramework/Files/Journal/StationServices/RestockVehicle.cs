namespace Observatory.Framework.Files.Journal
{
    public class RestockVehicle : JournalBase
    {
        public string Type { get; init; }
        public string Loadout { get; init; }
        public int Cost { get; init; }
        public int Count { get; init; }
    }
}
