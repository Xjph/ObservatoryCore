namespace Observatory.Framework.Files.Journal
{
    public class SquadronStartup : SquadronCreated
    {
        public int CurrentRank { get; init; }
        public string CurrentRankName { get; init; }
        public string CurrentRankName_Localised { get; init; }
        public ulong SquadronID { get; init; }
    }
}
