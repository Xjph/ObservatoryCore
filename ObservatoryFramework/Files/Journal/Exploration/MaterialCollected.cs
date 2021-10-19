namespace Observatory.Framework.Files.Journal
{
    public class MaterialCollected : JournalBase
    {
        public string Category { get; init; }
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public int Count { get; init; }
    }
}
