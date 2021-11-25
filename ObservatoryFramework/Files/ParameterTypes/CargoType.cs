namespace Observatory.Framework.Files.ParameterTypes
{
    public class CargoType
    {
        public string Name { get; init; }

        public string Name_Localised { get; init; }

        public int Count { get; init; }

        public int Stolen { get; init; }

        public ulong? MissionID { get; init; }
    }
}
