using System;
using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Docked : JournalBase
    {
        /// <summary>
        /// Name of the station at which this event occurred.
        /// </summary>
        public string StationName { get; init; }
        public string StationType { get; init; }
        public string StarSystem { get; init; }
        public ulong SystemAddress { get; init; }
        public ulong MarketID { get; init; }

        [JsonConverter(typeof(Converters.LegacyFactionConverter<Faction>))]
        public Faction StationFaction { get; init; }

        [Obsolete(JournalUtilities.ObsoleteMessage), JsonConverter(typeof(Converters.LegacyFactionConverter<Faction>))]
        public Faction Faction 
        {
            private get
            {
                return StationFaction;
            } 
            init 
            { 
                StationFaction = value; 
            } 
        }

        [Obsolete(JournalUtilities.ObsoleteMessage)]
        public string FactionState
        {
            private get
            {
                return StationFaction.FactionState;
            }

            init
            {
                //Stale Data, discard
            }
        }

        public string StationGovernment { get; init; }

        [Obsolete(JournalUtilities.ObsoleteMessage)]
        public string Government
        {
            private get { return StationGovernment; }
            init { StationGovernment = value; }
        }
        public string StationGovernment_Localised { get; init; }

        [Obsolete(JournalUtilities.ObsoleteMessage)]
        public string Government_Localised
        {
            private get { return StationGovernment_Localised; }
            init { StationGovernment_Localised = value; }
        }
        public string StationAllegiance { get; init; }

        [Obsolete(JournalUtilities.ObsoleteMessage)]
        public string Allegiance
        {
            private get { return StationAllegiance; }
            init { StationAllegiance = value; }
        }

        [JsonConverter(typeof(Converters.StationServiceConverter))]
        public StationService StationServices { get; init; }
        public string StationEconomy { get; init; }

        [Obsolete(JournalUtilities.ObsoleteMessage)]
        public string Economy
        {
            private get { return StationEconomy; }
            init { StationEconomy = value; }
        }
        public string StationEconomy_Localised { get; init; }

        [Obsolete(JournalUtilities.ObsoleteMessage)]
        public string Economy_Localised
        {
            private get { return StationEconomy_Localised; }
            init { StationEconomy_Localised = value; }
        }
        public ImmutableList<StationEconomy> StationEconomies { get; init; }

        [Obsolete("StationState is a rundundant property. Use StationEconomy to potentially reduce unnecessary checks.")]
        public string StationState { get; init; }
        public float DistFromStarLS { get; init; }
        public bool Wanted { get; init; }
        public bool ActiveFine { get; init; }
        public bool CockpitBreach { get; init; }
        public bool Taxi { get; init; }
        public bool Multicrew { get; init; }
    }
}
