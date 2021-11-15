﻿using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    /// <summary>
    /// Elite Dangerous shipyard.json file.
    /// </summary>
    public class ShipyardFile : Journal.JournalBase
    {
        /// <summary>
        /// Unique ID of market.
        /// </summary>
        public long MarketID { get; init; }
        /// <summary>
        /// Name of station where shipyard is located.
        /// </summary>
        public string StationName { get; init; }
        /// <summary>
        /// Starsystem where shipyard is located.
        /// </summary>
        public string StarSystem { get; init; }
        /// <summary>
        /// Whether player has access to Horizons content.
        /// </summary>
        public bool Horizons { get; init; }
        /// <summary>
        /// <para>Whether player has access to the Cobra MkIV.</para>
        /// <para>Will never be set to true for CMDR Nuse.</para>
        /// </summary>
        public bool AllowCobraMkIV { get; init; }
        /// <summary>
        /// List of all ships and prices for them at the current shipyard.
        /// </summary>
        public ImmutableList<ShipyardPrice> PriceList { get; init; }
    }
}
