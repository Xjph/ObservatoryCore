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
        [JsonPropertyName("Owned_Ship_Count")]
        public int OwnedShipCount { get; init; }
        [JsonPropertyName("Premium_Stock_Bought")]
        public int PremiumStockBought {  get; init; }
        [JsonPropertyName("Spent_On_Premium_Stock")]
        public long SpentOnPremiumStock { get; init; }
        [JsonPropertyName("Spent_On_Suit_Consumables")]
        public long SpentOnSuitConsumables { get; init; }
        [JsonPropertyName("Spent_On_Suits")]
        public long SpentOnSuits { get; init; }
        [JsonPropertyName("Spent_On_Weapons")]
        public long SpentOnWeapons { get; init; }
        [JsonPropertyName("Suits_Owned")]
        public int SuitsOwned { get; init; }
        [JsonPropertyName("Weapons_Owned")]
        public int WeaponsOwned { get; init; }
    }
}
