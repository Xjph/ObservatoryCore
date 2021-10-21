namespace Observatory.Framework.Files.ParameterTypes
{
    public class StoredItem
    {
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public string StarSystem { get; init; }
        public long MarketID { get; init; }
        public int StorageSlot { get; init; }
        public long TransferCost { get; init; }
        public long TransferTime { get; init; }
        public bool Hot { get; init; }
        public string EngineerModifications { get; init; }
        public int Level { get; init; }
        public float Quality { get; init; }
        public bool InTransit { get; init; }
    }
}
