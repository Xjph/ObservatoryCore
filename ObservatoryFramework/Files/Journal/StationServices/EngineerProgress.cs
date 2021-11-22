using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json.Serialization;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class EngineerProgress : JournalBase
    {
        public string Engineer { get; init; }
        public ulong EngineerID { get; init; }
        public int Rank { get; init; }
        public int RankProgress { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ParameterTypes.Progress Progress { get; init; }
        public ImmutableList<EngineerType> Engineers { get; init; }
    }
}
