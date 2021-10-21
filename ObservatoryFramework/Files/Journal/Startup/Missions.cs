using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Missions : JournalBase
    {
        public ImmutableList<Mission> Active { get; init; }
        public ImmutableList<Mission> Failed { get; init; }
        public ImmutableList<Mission> Complete { get; init; }
    }
}
