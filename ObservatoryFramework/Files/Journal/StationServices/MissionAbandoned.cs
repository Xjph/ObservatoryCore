﻿namespace Observatory.Framework.Files.Journal
{
    public class MissionAbandoned : JournalBase
    {
        public string Name { get; init; }
        public ulong MissionID { get; init; }
        public long Fine { get; init; }
    }
}
