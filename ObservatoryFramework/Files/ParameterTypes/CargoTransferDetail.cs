using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class CargoTransferDetail
    {
        public string Type { get; init; }
        public string Type_Localised { get; init; }
        public int Count { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CargoTransferDirection Direction { get; init; }
    }
}
