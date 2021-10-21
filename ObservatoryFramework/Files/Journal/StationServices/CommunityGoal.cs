using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class CommunityGoal : JournalBase
    {
        public ImmutableList<CurrentGoal> CurrentGoals { get; init; }
    }
}
