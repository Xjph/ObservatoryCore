namespace Observatory.Framework.Files.Journal
{
    public class Touchdown : JournalBase
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public string NearestDestination { get; init; }
        public string NearestDestination_Localised { get; init; }
        public bool PlayerControlled { get; init; }
        public bool Taxi { get; init; }
        public bool Multicrew { get; init; }
        public string StarSystem { get; init; }
        public ulong SystemAddress { get; init; }
        public string Body { get; init; }
        public int BodyID { get; init; }
        public bool OnStation { get; init; }
        public bool OnPlanet { get; init; }
    }
}
