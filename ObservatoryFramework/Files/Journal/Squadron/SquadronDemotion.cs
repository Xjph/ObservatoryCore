namespace Observatory.Framework.Files.Journal
{
    public class SquadronDemotion : SquadronCreated
    {
        public ulong SquadronID { get; init; }
        public int OldRank { get; init; }
        public int NewRank { get; init; }
        public string OldRankName { get; init; }
        public string OldRankName_Localised { get; init; }
        public string NewRankName { get; init; }
        public string NewRankName_Localised { get; init; }
    }
}
