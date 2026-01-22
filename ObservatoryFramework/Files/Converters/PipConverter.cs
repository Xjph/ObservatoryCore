using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Converters
{
    class PipConverter : JsonConverter<Pips>
    {
        public override Pips Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int[] values = (int[])JsonSerializer.Deserialize(ref reader, typeof(int[]));

            return new Pips() { Sys = values[0], Eng = values[1], Wep = values[2] };
        }

        public override void Write(Utf8JsonWriter writer, Pips value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new int[] { value.Sys, value.Eng, value.Wep }, options);
        }
    }
}
