using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Died : JournalBase
    {
        public string KillerName { get; init; }
        public string KillerName_Localised { get; init; }
        public string KillerShip { get; init; }
        public string KillerRank { get; init; }
        public ImmutableList<Killer> Killers { get; init; }
    }
}
