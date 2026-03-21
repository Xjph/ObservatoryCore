using System.Collections.Immutable;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class Cargo : JournalBase
    {
        public string Vessel { get; init; }
        public int Count { get; init; }
        public ImmutableList<CargoType> Inventory { get; init; }
    }
}
