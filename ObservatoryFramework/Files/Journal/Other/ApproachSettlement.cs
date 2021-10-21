namespace Observatory.Framework.Files.Journal
{
    public class ApproachSettlement : JournalBase
    {
        public ulong SystemAddress { get; init; }
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public long MarketID { get; init; }
        public float Latitude { get; init; }
        public float Longitude { get; init; }
        public int BodyID { get; init; }
        public string BodyName { get; init; }
    }
}
