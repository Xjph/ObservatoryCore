using Observatory.Framework.Files.Converters;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Route
    {
        public string StarSystem { get; init; }
        public ulong SystemAddress { get; init; }
        [JsonConverter(typeof(StarPosConverter))]
        public StarPosition StarPos { get; init; }
        public string StarClass { get; init; }
    }
}
