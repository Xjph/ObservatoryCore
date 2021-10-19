namespace Observatory.Framework.Files.Journal
{
    public class Powerplay : JournalBase
    {
        public string Power { get; init; }

        public int Rank { get; init; }

        public int Merits { get; init; }

        public int Votes { get; init; }

        public long TimePledged { get; init; }
    }
}
