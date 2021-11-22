using System;

namespace Observatory.Framework.Files.Journal
{
    public class ModuleStore : JournalBase
    {
        public ulong MarketID { get; init; }
        public string Slot { get; init; }
        public string Ship { get; init; }
        public ulong ShipID { get; init; }
        public string StoredItem { get; init; }
        public string StoredItem_Localised { get; init; }
        public bool Hot { get; init; }
        public string EngineerModifications { get; init; }
        public int Level { get; init; }
        public float Quality { get; init; }
        [Obsolete(JournalUtilities.UnusedMessage)]
        public string ReplacementItem { get; init; }
        [Obsolete(JournalUtilities.UnusedMessage)]
        public int Cost { get; init; }
    }
}
