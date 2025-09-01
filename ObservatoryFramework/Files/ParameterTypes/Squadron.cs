using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Squadron
    {
        [JsonPropertyName("Squadron_Bank_Credits_Deoposited")]
        public int SquadronBankCreditsDeposited { get; init; }
        [JsonPropertyName("Squadron_Bank_Credits_Withdrawn")]
        public int SquadronBankCreditsWithdrawn { get; init; }
        [JsonPropertyName("Squadron_Bank_Commodities_Deposited_Num")]
        public int SquadronBankCommoditiesDepositedNum { get; init; }
        [JsonPropertyName("Squadron_Bank_Commodities_Deposited_Value")]
        public int SquadronBankCommoditiesDepositedValue { get; init; }
        [JsonPropertyName("Squadron_Bank_Commodities_Withdrawn_Num")]
        public int SquadronBankCommoditiesWithdrawnNum { get; init; }
        [JsonPropertyName("Squadron_Bank_Commodities_Withdrawn_Value")]
        public int SquadronBankCommoditiesWithdrawnValue { get; init; }
        [JsonPropertyName("Squadron_Bank_PersonalAssets_Deposited_Num")]
        public int SquadronBankPersonalAssetsDepositedNum { get; init; }
        [JsonPropertyName("Squadron_Bank_PersonalAssets_Deposited_Value")]
        public int SquadronBankPersonalAssetsDepositedValue { get; init; }
        [JsonPropertyName("Squadron_Bank_PersonalAssets_Withdrawn_Num")]
        public int SquadronBankPersonalAssetsWithdrawnNum { get; init; }
        [JsonPropertyName("Squadron_Bank_PersonalAssets_Withdrawn_Value")]
        public int SquadronBankPersonalAssetsWithdrawnValue { get; init; }
        [JsonPropertyName("Squadron_Bank_Ships_Deposited_Num")]
        public int SquadronBankShipsDepositedNum { get; init; }
        [JsonPropertyName("Squadron_Bank_Ships_Deposited_Value")]
        public int SquadronBankShipsDepositedValue { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_aegis_highestcontribution")]
        public int SquadronLeaderboardAegisHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_bgs_highestcontribution")]
        public int SquadronLeaderboardBGSHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_bounty_highestcontribution")]
        public int SquadronLeaderboardBountyHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_colonisation_contribution_highestcontribution")]
        public int SquadronLeaderboardColonisationContributionHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_combat_highestcontribution")]
        public int SquadronLeaderboardCombatHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_cqc_highestcontribution")]
        public int SquadronLeaderboardCQCHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_exploration_highestcontribution")]
        public int SquadronLeaderboardExplorationHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_mining_highestcontribution")]
        public int SquadronLeaderboardMiningHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_powerplay_highestcontribution")]
        public int SquadronLeaderboardPowerplayHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_trade_highestcontribution")]
        public int SquadronLeaderboardTradeHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_trade_illicit_highestcontribution")]
        public int SquadronLeaderboardTradeIllicitHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_podiums")]
        public int SquadronLeaderboardPodiums { get; init; }
    }
}
