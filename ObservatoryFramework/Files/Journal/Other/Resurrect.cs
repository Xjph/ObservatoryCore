namespace Observatory.Framework.Files.Journal
{
    public class Resurrect : JournalBase
    {
        public string Option { get; init; }
        public int Cost { get; init; }
        public bool Bankrupt { get; init; }
    }
}
