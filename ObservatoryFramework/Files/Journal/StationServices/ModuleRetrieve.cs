namespace Observatory.Framework.Files.Journal
{
    public class ModuleRetrieve : JournalBase
    {
        public ulong MarketID { get; init; }
        public string Slot { get; init; }
        public string Ship { get; init; }
        public ulong ShipID { get; init; }
        public string RetrievedItem { get; init; }
        public string RetrievedItem_Localised { get; init; }
        public bool Hot { get; init; }
        public string SwapOutItem { get; init; }
        public string SwapOutItem_Localised { get; init; }
        public string EngineerModifications { get; init; }
        public int Level { get; init; }
        public float Quality { get; init; }
        public int Cost { get; init; }
    }
}
