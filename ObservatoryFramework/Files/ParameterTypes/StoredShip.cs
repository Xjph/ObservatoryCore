namespace Observatory.Framework.Files.ParameterTypes
{
    public class StoredShip
    {
        public ulong ShipID { get; init; }
        public string ShipType { get; init; }
        public string Name { get; init; }
        public long Value { get; init; }
        public bool Hot { get; init; }
        public bool InTransit { get; init; }
        public string StarSystem { get; init; }
        public long ShipMarketID { get; init; }
        public long TransferPrice { get; init; }
        public long TransferTime { get; init; }
    }
}
