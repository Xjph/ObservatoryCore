using System;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Crafting
    {
        [JsonPropertyName("Spent_On_Crafting"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public long SpentOnCrafting { get; init; }

        [JsonPropertyName("Count_Of_Used_Engineers")]
        public int CountOfUsedEngineers { get; init; }

        [JsonPropertyName("Recipes_Generated")]
        public int RecipesGenerated { get; init; }

        [JsonPropertyName("Recipes_Generated_Rank_1")]
        public int RecipesGeneratedRank1 { get; init; }

        [JsonPropertyName("Recipes_Generated_Rank_2")]
        public int RecipesGeneratedRank2 { get; init; }

        [JsonPropertyName("Recipes_Generated_Rank_3")]
        public int RecipesGeneratedRank3 { get; init; }

        [JsonPropertyName("Recipes_Generated_Rank_4")]
        public int RecipesGeneratedRank4 { get; init; }

        [JsonPropertyName("Recipes_Generated_Rank_5")]
        public int RecipesGeneratedRank5 { get; init; }

        [JsonPropertyName("Suit_Mods_Applied")]
        public int SuitModsApplied { get; init; }
        
        [JsonPropertyName("Suit_Mods_Applied_Full")]
        public int SuitModsAppliedFull { get; init; }

        [JsonPropertyName("Suits_Upgraded")]
        public int SuitsUpgraded { get; init; }

        [JsonPropertyName("Suits_Upgraded_Full")]
        public int SuitsUpgradedFull { get; init; }

        [JsonPropertyName("Weapon_Mods_Applied")]
        public int WeaponModsApplied { get; init; }

        [JsonPropertyName("Weapon_Mods_Applied_Full")]
        public int WeaponModsAppliedFull { get; init; }

        [JsonPropertyName("Weapons_Upgraded")]
        public int WeaponsUpgraded { get; init; }

        [JsonPropertyName("Weapons_Upgraded_Full")]
        public int WeaponsUpgradedFull { get; init; }

        [JsonPropertyName("Recipes_Applied"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public int RecipesApplied { get; init; }

        [JsonPropertyName("Recipes_Applied_Rank_1"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public int RecipesAppliedRank1 { get; init; }

        [JsonPropertyName("Recipes_Applied_Rank_2"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public int RecipesAppliedRank2 { get; init; }

        [JsonPropertyName("Recipes_Applied_Rank_3"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public int RecipesAppliedRank3 { get; init; }

        [JsonPropertyName("Recipes_Applied_Rank_4"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public int RecipesAppliedRank4 { get; init; }

        [JsonPropertyName("Recipes_Applied_Rank_5"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public int RecipesAppliedRank5 { get; init; }

        [JsonPropertyName("Recipes_Applied_On_Previously_Modified_Modules"), Obsolete(JournalUtilities.ObsoleteMessage)]
        public int RecipesAppliedOnPreviouslyModifiedModules { get; init; }
    }
}
