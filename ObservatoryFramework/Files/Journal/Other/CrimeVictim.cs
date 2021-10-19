using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CrimeVictim : JournalBase
    {
        public string Offender { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CrimeType CrimeType { get; init; }
        public int Fine { get; init; }
        public int Bounty { get; init; }
    }
}
