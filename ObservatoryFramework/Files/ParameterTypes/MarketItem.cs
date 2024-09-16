﻿namespace Observatory.Framework.Files.ParameterTypes
{
    public class MarketItem
    {
        public ulong id { get; init; }
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public string Category { get; init; }
        public string Category_Localised { get; init; }
        public uint BuyPrice { get; init; }
        public uint SellPrice { get; init; }
        public int StockBracket { get; init; }
        public int DemandBracket { get; init; }
        public int Stock { get; init; }
        public int Demand { get; init; }
        public bool Consumer { get; init; }
        public bool Producer { get; init; }
        public bool Rare { get; init; }
    }
}
