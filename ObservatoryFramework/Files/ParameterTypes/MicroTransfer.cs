using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class MicroTransfer : MicroResource
    {
        public MicroTransferDirection Direction { get; init; }
    }
}
