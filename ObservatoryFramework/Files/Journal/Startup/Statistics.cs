using System;
using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class Statistics : JournalBase
    {
        [JsonPropertyName("Bank_Account")]
        public BankAccount BankAccount { get; init; }
        public Combat Combat { get; init; }
        public Crime Crime { get; init; }
        public Smuggling Smuggling { get; init; }
        public Trading Trading { get; init; }
        public Mining Mining { get; init; }
        public Exploration Exploration { get; init; }
        public Passengers Passengers { get; init; }
        [JsonPropertyName("Search_And_Rescue")]
        public ParameterTypes.SearchAndRescue SearchAndRescue { get; init; }
        public Crafting Crafting { get; init; }
        public Crew Crew { get; init; }
        public Multicrew Multicrew { get; init; }
        [JsonPropertyName("TG_ENCOUNTERS")]
        public Thargoid Thargoid { get; init; }
        [JsonPropertyName("Material_Trader_Stats")]
        public MaterialTrader MaterialTrader { get; init; }
        public CQC CQC { get; init; }
        [JsonPropertyName("FLEETCARRIER")]
        public FleetCarrier FleetCarrier { get; init; }
        public Exobiology Exobiology { get; init; }
        public Squadron Squadron { get; init; }
    }
}
