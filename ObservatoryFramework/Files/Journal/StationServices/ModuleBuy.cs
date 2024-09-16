﻿namespace Observatory.Framework.Files.Journal
{
    public class ModuleBuy : JournalBase
    {
        public ulong MarketID { get; init; }
        public string Slot { get; init; }
        public string BuyItem { get; init; }
        public string BuyItem_Localised { get; init; }
        public uint BuyPrice { get; init; }
        public string SellItem { get; init; }
        public string SellItem_Localised { get; init; }
        public uint SellPrice { get; init; }
        public string StoredItem { get; init; }
        public string StoredItem_Localised { get; init; }
        public string Ship { get; init; }
        public ulong ShipID { get; init; }
    }
}
