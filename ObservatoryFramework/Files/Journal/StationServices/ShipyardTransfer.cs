﻿namespace Observatory.Framework.Files.Journal
{
    public class ShipyardTransfer : JournalBase
    {
        public ulong MarketID { get; init; }
        public string ShipType { get; init; }
        public string ShipType_Localised { get; init; }
        public ulong ShipID { get; init; }
        public string System { get; init; }
        public ulong ShipMarketID { get; init; }
        public float Distance { get; init; }
        public int TransferPrice { get; init; }
        public long TransferTime { get; init; }
    }
}
