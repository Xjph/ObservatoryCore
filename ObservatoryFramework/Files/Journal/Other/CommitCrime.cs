using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CommitCrime : JournalBase
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CrimeType CrimeType { get; init; }
        public string Faction { get; init; }
        public string Victim { get; init; }
        public string Victim_Localised { get; init; }
        public int Fine { get; init; }
        public int Bounty { get; init; }
    }
}
