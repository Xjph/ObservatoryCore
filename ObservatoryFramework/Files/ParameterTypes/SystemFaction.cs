using System.Collections.Immutable;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class SystemFaction : Faction
    {
        public string Government { get; init; }

        public double Influence { get; init; }

        public string Happiness { get; init; }

        public string Happiness_Localised { get; init; }

        public double MyReputation { get; init; }

        public ImmutableList<FactionStateTrend> PendingStates { get; init; }

        public ImmutableList<FactionStateTrend> RecoveringStates { get; init; }

        public ImmutableList<FactionState> ActiveStates { get; init; }

        public bool? SquadronFaction { get; init; }

        public bool? HappiestSystem { get; init; }

        public bool? HomeSystem { get; init; }
    }
}
