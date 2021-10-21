using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class NpcCrewRank : JournalBase
    {
        public int NpcCrewId { get; init; }
        public string NpcCrewName { get; init; }
        public RankCombat RankCombat { get; init; }
    }
}
