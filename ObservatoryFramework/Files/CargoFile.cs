using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    public class CargoFile : Journal.JournalBase
    {
        public string Vessel { get; init; }
        public int Count { get; init; }
        public ImmutableList<CargoType> Inventory { get; init; }
    }
}
