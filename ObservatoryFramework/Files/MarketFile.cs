using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    /// <summary>
    /// Elite Dangerous market.json file. Contains details about all available commodities available at the current station.
    /// </summary>
    public class MarketFile : Journal.JournalBase
    {
        /// <summary>
        /// Unique ID of current market.
        /// </summary>
        public ulong MarketID { get; init; }
        /// <summary>
        /// Name of the station where the market is located.
        /// </summary>
        public string StationName { get; init; }
        /// <summary>
        /// Type of station where the market is located.
        /// </summary>
        public string StationType { get; init; }
        /// <summary>
        /// Name of star system where the market is located.
        /// </summary>
        public string StarSystem { get; init; }
        /// <summary>
        /// List of all commodities available in the market.
        /// </summary>
        public ImmutableList<MarketItem> Items { get; init; }
    }
}
