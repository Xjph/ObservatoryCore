using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    /// <summary>
    /// Elite Dangerous outfitting.json file. Contains data about ship modules available for purchase at the current station.
    /// </summary>
    public class OutfittingFile : Journal.JournalBase
    {
        /// <summary>
        /// Unique ID of current market.
        /// </summary>
        public long MarketID { get; init; }
        /// <summary>
        /// Name of the station where the market is located.
        /// </summary>
        public string StationName { get; init; }
        /// <summary>
        /// Name of the star system where the market is located.
        /// </summary>
        public string StarSystem { get; init; }
        /// <summary>
        /// Indicator if the player has access to Horizons content.
        /// </summary>
        public bool Horizons { get; init; }
        /// <summary>
        /// List of all available parts in shipyard.
        /// </summary>
        public ImmutableList<OutfittingModule> Items { get; init; }
    }
}
