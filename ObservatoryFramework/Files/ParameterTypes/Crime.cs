using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Crime
    {
        public int Notoriety { get; init; }

        public long Fines { get; init; }

        [JsonPropertyName("Total_Fines")]
        public long TotalFines { get; init; }

        [JsonPropertyName("Bounties_Received")]
        public int BountiesReceived { get; init; }

        [JsonPropertyName("Total_Bounties")]
        public decimal TotalBounties { get; init; }

        [JsonPropertyName("Highest_Bounty")]
        public decimal HighestBounty { get; init; }
        
        [JsonPropertyName("Citizens_Murdered")]
        public int CitizensMurdered { get; init; }

        [JsonPropertyName("Data_Stolen")]
        public int DataStolen {  get; init; }

        [JsonPropertyName("Goods_Stolen")]
        public int GoodsStolen { get; init; }

        [JsonPropertyName("Guards_Murdered")]
        public int GuardsMurdered { get; init; }

        [JsonPropertyName("Malware_Uploaded")]
        public int MalwareUploaded { get; init; }

        [JsonPropertyName("Omnipol_Murdered")]
        public int OmnipolMurdered { get; init; }

        [JsonPropertyName("Production_Sabotage")]
        public int ProductionSabotage { get; init; }

        [JsonPropertyName("Production_Theft")]
        public int ProductionTheft {  get; init; }

        [JsonPropertyName("Profiles_Cloned")]
        public int ProfilesCloned { get; init; }

        [JsonPropertyName("Sample_Stolen")]
        public int SampleStolen { get; init; }

        [JsonPropertyName("Settlements_State_Shutdown")]
        public int SettlementsStateShutdown { get; init; }

        [JsonPropertyName("Total_Murders")]
        public int TotalMurders { get; init; }

        [JsonPropertyName("Total_Stolen")]
        public int TotalStolen { get; init; }

        [JsonPropertyName("Turrets_Destroyed")]
        public int TurretsDestroyed { get; init; }

        [JsonPropertyName("Turrets_Overloaded")]
        public int TurretsOverloaded { get; init; }

        [JsonPropertyName("Turrets_Total")]
        public int TurretsTotal { get; init; }

        [JsonPropertyName("Value_Stolen_StateChange")]
        public int ValueStolenStatechange { get; init; }
    }
}
