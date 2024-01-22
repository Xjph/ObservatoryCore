using Microsoft.VisualBasic.CompilerServices;
using Observatory.Framework.Files.Journal;
using System;
using System.Numerics;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class CurrentGoal
    {
        public long CGID { get; init; }
        public string Title { get; init; }
        public string SystemName { get; init; }
        public string MarketName { get; init; }
        public string Expiry { get; init; }
        public DateTime ExpiryDateTime
        {
            get => JournalBase.ParseDateTime(Expiry);
        }
        public bool IsComplete { get; init; }
        public long CurrentTotal { get; init; }
        public long PlayerContribution { get; init; }
        public long NumContributors { get; init; }
        public int PlayerPercentileBand { get; init; }
        public TopTier TopTier { get; init; }
        public int TopRankSize { get; init; }
        public bool PlayerInTopRank { get; init; }
        public string TierReached { get; init; }
        public long Bonus { get; init; }

    }
}
