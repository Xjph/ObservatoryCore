using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Converters
{
    /// <summary>
    /// Converting the ordered array of coordinates from the journal to a named tuple for clarity.
    /// </summary>
    public class StarPosConverter : JsonConverter<StarPosition>
    {
        public override StarPosition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            double[] values = (double[])JsonSerializer.Deserialize(ref reader, typeof(double[]));

            return new StarPosition() { x = values[0], y = values[1], z = values[2] };
        }

        public override void Write(Utf8JsonWriter writer, StarPosition value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
