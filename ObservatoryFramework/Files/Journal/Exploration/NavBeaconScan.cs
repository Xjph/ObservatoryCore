namespace Observatory.Framework.Files.Journal
{
    public class NavBeaconScan : JournalBase
    {
        public int NumBodies { get; init; }
        public ulong SystemAddress { get; init; }
    }
}
