using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class ApproachSettlement : JournalBase
    {
        public ulong SystemAddress { get; init; }
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public ulong MarketID { get; init; }
        public float Latitude { get; init; }
        public float Longitude { get; init; }
        public int BodyID { get; init; }
        public string BodyName { get; init; }
        public ImmutableList<StationEconomy> StationEconomies { get; init; }
        public string StationEconomy { get; init; }
        public string StationEconomy_Localised { get; init; }
        public Faction StationFaction { get; init; }
        public string StationGovernment { get; init; }
        public string StationGovernment_Localised { get; init; }
        public StationService StationServices { get; init; }
    }
}
