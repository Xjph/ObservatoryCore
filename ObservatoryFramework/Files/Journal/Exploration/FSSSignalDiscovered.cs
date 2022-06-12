namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated when a signal source is identified or scanned.
    /// </summary>
    public class FSSSignalDiscovered : JournalBase
    {
        /// <summary>
        /// Name of the signal type.
        /// </summary>
        public string SignalName { get; init; }
        /// <summary>
        /// Localised name of the signal type.
        /// </summary>
        public string SignalName_Localised { get; init; }
        /// <summary>
        /// Faction state or circumstance that caused this signal to appear.
        /// </summary>
        public string SpawningState { get; init; }
        /// <summary>
        /// Localised description of spawning state.
        /// </summary>
        public string SpawningState_Localised { get; init; }
        /// <summary>
        /// Faction name which is associated with this signal.
        /// </summary>
        public string SpawningFaction { get; init; }
        /// <summary>
        /// Localised name of the associated faction.
        /// </summary>
        public string SpawningFaction_Localised { get; init; }
        /// <summary>
        /// Time until the signal despawns, in seconds.
        /// </summary>
        public float TimeRemaining { get; init; }
        /// <summary>
        /// Unique ID of current system.
        /// </summary>
        public ulong SystemAddress { get; init; }
        /// <summary>
        /// Numeric representation of the signal threat level.
        /// </summary>
        public int ThreatLevel { get; init; }
        /// <summary>
        /// Type of signal.
        /// </summary>
        public string USSType { get; init; }
        /// <summary>
        /// Localised name of signal type.
        /// </summary>
        public string USSType_Localised { get; init; }
        /// <summary>
        /// Indicator if the signal is a station which can be docked with.
        /// </summary>
        public bool IsStation { get; init; }
    }
}
