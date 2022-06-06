namespace Observatory.Framework.Files.Journal
{
    public class BookTaxi : JournalBase
    {
        public int Cost { get; init; }
        public string DestinationSystem { get; init; }
        public string DestinationLocation { get; init; }
        public bool Retreat { get; init; }
    }
}
