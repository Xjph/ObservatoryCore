using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class BankAccount
    {
        [JsonPropertyName("Current_Wealth")]
        public long CurrentWealth { get; init; }

        [JsonPropertyName("Spent_On_Ships")]
        public long SpentOnShips { get; init; }

        [JsonPropertyName("Spent_On_Outfitting")]
        public long SpentOnOutfitting { get; init; }

        [JsonPropertyName("Spent_On_Repairs")]
        public long SpentOnRepairs { get; init; }

        [JsonPropertyName("Spent_On_Fuel")]
        public long SpentOnFuel { get; init; }

        [JsonPropertyName("Spent_On_Ammo_Consumables")]
        public long SpentOnAmmoConsumables { get; init; }

        [JsonPropertyName("Insurance_Claims")]
        public int InsuranceClaims { get; init; }

        [JsonPropertyName("Spent_On_Insurance")]
        public long SpentOnInsurance { get; init; }
    }
}
