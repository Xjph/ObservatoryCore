using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Modifiers
    {
        public string Label { get; init; }

        public double Value { get; init; }

        public double OriginalValue { get; init; }

        [JsonConverter(typeof(Converters.IntBoolConverter))]
        public bool LessIsGood { get; init; }
    }
}
