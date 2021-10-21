using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class Scanned : JournalBase
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ScanType ScanType { get; init; }
    }
}
