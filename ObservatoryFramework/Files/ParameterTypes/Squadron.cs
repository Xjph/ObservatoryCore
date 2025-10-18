using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Squadron
    {
        [JsonPropertyName("Squadron_Bank_Credits_Deoposited")]
        public long SquadronBankCreditsDeposited { get; init; }
        [JsonPropertyName("Squadron_Bank_Credits_Withdrawn")]
        public long SquadronBankCreditsWithdrawn { get; init; }
        [JsonPropertyName("Squadron_Bank_Commodities_Deposited_Num")]
        public int SquadronBankCommoditiesDepositedNum { get; init; }
        [JsonPropertyName("Squadron_Bank_Commodities_Deposited_Value")]
        public long SquadronBankCommoditiesDepositedValue { get; init; }
        [JsonPropertyName("Squadron_Bank_Commodities_Withdrawn_Num")]
        public int SquadronBankCommoditiesWithdrawnNum { get; init; }
        [JsonPropertyName("Squadron_Bank_Commodities_Withdrawn_Value")]
        public long SquadronBankCommoditiesWithdrawnValue { get; init; }
        [JsonPropertyName("Squadron_Bank_PersonalAssets_Deposited_Num")]
        public int SquadronBankPersonalAssetsDepositedNum { get; init; }
        [JsonPropertyName("Squadron_Bank_PersonalAssets_Deposited_Value")]
        public long SquadronBankPersonalAssetsDepositedValue { get; init; }
        [JsonPropertyName("Squadron_Bank_PersonalAssets_Withdrawn_Num")]
        public int SquadronBankPersonalAssetsWithdrawnNum { get; init; }
        [JsonPropertyName("Squadron_Bank_PersonalAssets_Withdrawn_Value")]
        public long SquadronBankPersonalAssetsWithdrawnValue { get; init; }
        [JsonPropertyName("Squadron_Bank_Ships_Deposited_Num")]
        public int SquadronBankShipsDepositedNum { get; init; }
        [JsonPropertyName("Squadron_Bank_Ships_Deposited_Value")]
        public long SquadronBankShipsDepositedValue { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_aegis_highestcontribution")]
        public long SquadronLeaderboardAegisHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_bgs_highestcontribution")]
        public long SquadronLeaderboardBGSHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_bounty_highestcontribution")]
        public long SquadronLeaderboardBountyHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_colonisation_contribution_highestcontribution")]
        public long SquadronLeaderboardColonisationContributionHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_combat_highestcontribution")]
        public long SquadronLeaderboardCombatHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_cqc_highestcontribution")]
        public long SquadronLeaderboardCQCHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_exploration_highestcontribution")]
        public long SquadronLeaderboardExplorationHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_mining_highestcontribution")]
        public long SquadronLeaderboardMiningHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_powerplay_highestcontribution")]
        public long SquadronLeaderboardPowerplayHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_trade_highestcontribution")]
        public long SquadronLeaderboardTradeHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_trade_illicit_highestcontribution")]
        public long SquadronLeaderboardTradeIllicitHighestContribution { get; init; }
        [JsonPropertyName("Squadron_Leaderboard_podiums")]
        public int SquadronLeaderboardPodiums { get; init; }
    }
}
