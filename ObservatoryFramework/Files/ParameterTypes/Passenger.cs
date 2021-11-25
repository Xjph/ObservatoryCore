namespace Observatory.Framework.Files.ParameterTypes
{
    public class Passenger
    {
        public ulong MissionID { get; init; }

        public string Type { get; init; }

        public bool VIP { get; init; }

        public bool Wanted { get; init; }

        public int Count { get; init; }
    }
}
