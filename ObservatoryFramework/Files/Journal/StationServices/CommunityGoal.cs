using System.Collections.Immutable;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CommunityGoal : JournalBase
    {
        public ImmutableList<CurrentGoal> CurrentGoals { get; init; }
    }
}
