using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CrimeVictim : JournalBase
    {
        public string Offender { get; init; }
        public string Offender_Localised { get; init; } 
        public CrimeType CrimeType { get; init; }
        public int Fine { get; init; }
        public int Bounty { get; init; }
    }
}
