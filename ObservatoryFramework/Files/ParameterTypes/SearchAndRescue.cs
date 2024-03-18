using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class SearchAndRescue
    {
        [JsonPropertyName("SearchRescue_Traded")]
        public int Traded { get; init; }

        [JsonPropertyName("SearchRescue_Profit")]
        public long Profit { get; init; }

        [JsonPropertyName("SearchRescue_Count")]
        public int Count { get; init; }

        [JsonPropertyName("Maglocks_Opened")]
        public int MaglocksOpened { get; init; }

        [JsonPropertyName("Panels_Opened")]
        public int PanelsOpened { get; init; }

        [JsonPropertyName("Salvage_Illegal_POI")]
        public int SalvageIllegalPoi { get; init; }

        [JsonPropertyName("Salvage_Illegal_Settlements")]
        public int SalvageIllegalSettlements { get; init; }

        [JsonPropertyName("Salvage_Legal_POI")]
        public int SalvageLegalPoi { get; init; }

        [JsonPropertyName("Salvage_Legal_Settlements")]
        public int SalvageLegalSettlements { get; init; }

        [JsonPropertyName("Settlements_State_FireOut")]
        public int SettlementsStateFireOut { get; init; }

        [JsonPropertyName("Settlements_State_Reboot")]
        public int SettlementsStateReboot { get; init; }
    }
}
