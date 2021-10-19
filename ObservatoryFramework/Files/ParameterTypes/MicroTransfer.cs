using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class MicroTransfer : MicroResource
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MicroTransferDirection Direction { get; init; }
    }
}
