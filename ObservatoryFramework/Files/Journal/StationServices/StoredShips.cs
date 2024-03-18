using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class StoredShips : JournalBase
    {
        public ulong MarketID { get; init; }
        /// <summary>
        /// Name of the station at which this event occurred.
        /// </summary>
        public string StationName { get; init; }
        public string StarSystem { get; init; }
        public ImmutableList<StoredShip> ShipsHere { get; init; }
        public ImmutableList<StoredShip> ShipsRemote { get; init; }
    }
}
