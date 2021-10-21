using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class EngineerType
    {
        public string Engineer { get; init; }
        public int EngineerID { get; init; }
        public int Rank { get; init; }
        public int RankProgress { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Progress Progress { get; init; }
    }
}
