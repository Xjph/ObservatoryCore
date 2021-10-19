namespace Observatory.Framework.Files.Journal
{
    public class SendText : JournalBase
    {
        public string To { get; init; }
        public string To_Localised { get; init; }
        public string Message { get; init; }
        public bool Sent { get; init; }
    }
}
