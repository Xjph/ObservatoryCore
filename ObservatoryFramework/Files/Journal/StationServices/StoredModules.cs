using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class StoredModules : JournalBase
    {
        public string StarSystem { get; init; }
        /// <summary>
        /// Name of the station at which this event occurred.
        /// </summary>
        public string StationName { get; init; }
        public ulong MarketID { get; init; }
        public ImmutableList<StoredItem> Items { get; init; }
    }
}
