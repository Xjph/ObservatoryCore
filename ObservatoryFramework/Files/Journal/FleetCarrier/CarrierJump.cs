using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierJump : FSDJump
    {
        public bool Docked { get; init; }
        public bool OnFoot { get; init; }
        /// <summary>
        /// Name of the station at which this event occurred.
        /// </summary>
        public string StationName { get; init; }
        public string StationType { get; init; }
        public ulong MarketID { get; init; }
        public Faction StationFaction { get; init; }
        public string StationGovernment { get; init; }
        public string StationGovernment_Localised { get; init; }
        public StationService StationServices { get; init; }
        public string StationEconomy { get; init; }
        public string StationEconomy_Localised { get; init; }
        public ImmutableList<StationEconomy> StationEconomies { get; init; }
    }
}
