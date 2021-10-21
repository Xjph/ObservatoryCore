using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class SAASignalsFound : JournalBase
    {
        public ulong SystemAddress { get; init; }
        public string BodyName { get; init; }
        public int BodyID { get; init; }
        public ImmutableList<Signal> Signals { get; init; }
    }
}
