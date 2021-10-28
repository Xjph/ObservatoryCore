namespace Observatory.Framework.Files.Journal
{
    public class UnhandledEvent : JournalBase
    {
        public string OriginalEvent { get; init; }
    }
}
