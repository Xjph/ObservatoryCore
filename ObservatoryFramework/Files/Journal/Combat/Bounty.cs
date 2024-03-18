using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Bounty : JournalBase
    {
        public ImmutableList<Rewards> Rewards { get; init; }
        public string PilotName { get; set; }
        public string PilotName_Localised { get; set; }
        public string Target { get; init; }
        public string Target_Localised { get; init; }
        public string Faction { get; init; }
        public string Faction_Localised { get; init; }
        public long Reward { get; init; }
        public long TotalReward { get; init; }
        public string VictimFaction { get; init; }
        public string VictimFaction_Localised { get; init; }
        public int SharedWithOthers { get; init; }
    }
}
