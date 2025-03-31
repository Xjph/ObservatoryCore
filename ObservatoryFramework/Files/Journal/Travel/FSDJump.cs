using System;
using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class FSDJump : JournalBase
    {
        public string StarSystem { get; init; }
        public ulong SystemAddress { get; init; }
        [JsonConverter(typeof(Converters.StarPosConverter))]
        public StarPosition StarPos { get; init; }
        public string Body { get; init; }
        public int BodyID { get; init; }
        public string BodyType { get; init; }
        public double JumpDist { get; init; }
        public double FuelUsed { get; init; }
        public double FuelLevel { get; init; }
        public int BoostUsed { get; init; }
        [JsonConverter(typeof(Converters.LegacyFactionConverter<SystemFaction>))]
        public SystemFaction SystemFaction { get; init; }
        [Obsolete(JournalUtilities.ObsoleteMessage)]
        public string FactionState
        {
            get
            {
                return SystemFaction.FactionState;
            }
            init
            {
                //Stale Data, discard
            }
        }
        public string SystemAllegiance { get; init; }
        public string SystemEconomy { get; init; }
        public string SystemEconomy_Localised { get; init; }
        public string SystemSecondEconomy { get; init; }
        public string SystemSecondEconomy_Localised { get; init; }
        public string SystemGovernment { get; init; }
        public string SystemGovernment_Localised { get; init; }
        public string SystemSecurity { get; init; }
        public string SystemSecurity_Localised { get; init; }
        public long Population { get; init; }
        public bool Wanted { get; init; }
        public ImmutableList<SystemFaction> Factions { get; init; }
        public ImmutableList<Conflict> Conflicts { get; init; }
        public ImmutableList<string> Powers { get; init; }
        public string PowerplayState { get; init; }
        public double PowerplayStateControlProgress { get; init; }
        public int PowerplayStateReinforcement { get; init; }
        public int PowerplayStateUndermining { get; init; }
        public ImmutableList<PowerplayStateConflictProgress> PowerplayConflictProgress { get; init; }
        public bool Taxi { get; init; }
        public bool Multicrew { get; init; }
        public ThargoidWar ThargoidWar { get; init; }
        public string Power { get; init; }
    }
}
