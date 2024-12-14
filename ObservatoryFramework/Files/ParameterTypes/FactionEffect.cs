using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class FactionEffect
    {
        public string Faction { get; init; }
        public ImmutableList<EffectType> Effects { get; init; }
        public ImmutableList<InfluenceType> Influence { get; init; }
        [JsonConverter(typeof(RepInfConverter))]
        public int Reputation { get; init; }
        public TrendValue ReputationTrend { get; init; }
    }
}
