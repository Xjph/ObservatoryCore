using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class LaunchDrone : JournalBase
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LimpetDrone Type { get; init; }
    }
}
