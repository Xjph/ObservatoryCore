namespace Observatory.Framework.Files.Journal
{
    public class BuyTradeData : JournalBase
    {
        public string System { get; init; }
        public long Cost { get; init; }
    }
}
