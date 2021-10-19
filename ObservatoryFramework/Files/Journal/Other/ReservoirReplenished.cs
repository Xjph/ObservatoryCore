namespace Observatory.Framework.Files.Journal
{
    public class ReservoirReplenished : JournalBase
    {
        public float FuelMain { get; init; }
        public float FuelReservoir { get; init; }
    }
}
