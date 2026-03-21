using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;

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
