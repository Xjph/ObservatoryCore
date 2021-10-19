namespace Observatory.Framework.Files.ParameterTypes
{
    public class BackpackItem
    {
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public ulong OwnerID { get; init; }
        public ulong MissionID { get; init; }
        public int Count { get; init; }
    }
}
