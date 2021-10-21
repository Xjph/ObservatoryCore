using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Passengers : JournalBase
    {
        public ImmutableList<Passenger> Manifest { get; init; }

    }
}
