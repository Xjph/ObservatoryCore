using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Cargo : JournalBase
    {
        /// <summary>
        /// Type of vehicle currently being reported. "Ship" or "SRV".
        /// </summary>
        public string Vessel { get; init; }
        /// <summary>
        /// Number of different types of cargo carried(?)
        /// </summary>
        public int Count { get; init; }
        /// <summary>
        /// List of full cargo details.
        /// </summary>
        public ImmutableList<CargoType> Inventory { get; init; }
    }
}