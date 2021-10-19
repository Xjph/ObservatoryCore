namespace Observatory.Framework.Files.Journal
{
    public class ShipTargeted : JournalBase
    {
        public bool TargetLocked { get; init; }
        public string Ship { get; init; }
        public string Ship_Localised { get; init; }
        public int ScanStage { get; init; }
        public string PilotName { get; init; }
        public string PilotName_Localised { get; init; }
        public string PilotRank { get; init; }
        public float ShieldHealth { get; init; }
        public float HullHealth { get; init; }
        public string Faction { get; init; }
        public string LegalStatus { get; init; }
        public long Bounty { get; init; }
        public string Subsystem { get; init; }
        public string Subsystem_Localised { get; init; }
        public float SubsystemHealth { get; init; }
        public string Power { get; init; }
        public string SquadronID { get; init; }
    }
}
