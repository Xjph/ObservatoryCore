namespace Observatory.Framework.Files.Journal.Startup
{
    public class GameModeChange : JournalBase
    {
        // Known values as of 2026/09: "MainGame", "Operation", "ProvingGrounds" (aka. CQC)
        public string GameMode { get; init; }
    }
}
