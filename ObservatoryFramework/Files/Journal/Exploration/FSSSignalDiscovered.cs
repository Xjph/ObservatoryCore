namespace Observatory.Framework.Files.Journal
{
    public class FSSSignalDiscovered : JournalBase
    {
        public string SignalName { get; init; }
        public string SignalName_Localised { get; init; }
        public string SpawningState { get; init; }
        public string SpawningState_Localised { get; init; }
        public string SpawningFaction { get; init; }
        public string SpawningFaction_Localised { get; init; }
        public float TimeRemaining { get; init; }
        public ulong SystemAddress { get; init; }
        public int ThreatLevel { get; init; }
        public string USSType { get; init; }
        public string USSType_Localised { get; init; }
        public bool IsStation { get; init; }
    }
}
