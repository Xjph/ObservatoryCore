using System;

namespace Observatory.Framework.Files.Journal
{
    [Obsolete(JournalUtilities.ObsoleteMessage)]
    public class PayLegacyFines : JournalBase
    {
        public long Amount { get; init; }
        public float BrokerPercentage { get; init; }
    }
}