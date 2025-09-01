﻿using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierBuy : JournalBase
    {
        public long BoughtAtMarket { get; init; }
        public ulong SystemAddress { get; init; }
        public ulong CarrierID { get; init; }
        public CarrierType CarrierType { get; init; }
        public string Location { get; init; }
        public long Price { get; init; }
        public string Variant { get; init; }
        public string Callsign { get; init; }
    }
}
