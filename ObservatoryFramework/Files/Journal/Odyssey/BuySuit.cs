namespace Observatory.Framework.Files.Journal
{
    public class BuySuit : JournalBase
    {
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public int Price { get; init; }
        public ulong SuitID { get; init; }
    }
}
