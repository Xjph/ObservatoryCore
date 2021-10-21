using Observatory.Framework.Files.Converters;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Modifier
    {
        public string Label { get; init; }
        public float Value { get; init; }
        public float OriginalValue { get; init; }
        [JsonConverter(typeof(IntBoolConverter))]
        public bool LessIsGood { get; init; }
        public string ValueStr { get; init; }
    }
}
