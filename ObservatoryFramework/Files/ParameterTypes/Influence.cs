using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class InfluenceType
    {
        public ulong SystemAddress { get; init; }
        public TrendValue Trend { get; init; }
        [JsonConverter(typeof(RepInfConverter))]
        public int Influence { get; init; }
    }
}
