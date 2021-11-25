namespace Observatory.Framework.Files.ParameterTypes
{
    public class Mission
    {
        public ulong MissionID { get; init; }

        public string Name { get; init; }

        public bool PassengerMission { get; init; }

        public int Expires { get; init; }
    }
}
