using System.Collections.Immutable;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class ShipLockerMaterials : JournalBase
    {
        public ImmutableList<BackpackItem> Items { get; init; }
        public ImmutableList<BackpackItem> Components { get; init; }
        public ImmutableList<BackpackItem> Consumables { get; init; }
        public ImmutableList<BackpackItem> Data { get; init; }
    }
}
