using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class PowerplayVote : PowerplayJoin
    {
        public int Votes { get; init; }
        [JsonPropertyName("")]
        public int UnnamedValue { get; init; }
        public string System { get; init; }
    }
}
