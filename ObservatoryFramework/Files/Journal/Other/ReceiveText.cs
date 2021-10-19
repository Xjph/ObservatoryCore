using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class ReceiveText : JournalBase
    {
        public string From { get; init; }
        public string From_Localised { get; init; }
        public string Message { get; init; }
        public string Message_Localised { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TextChannel Channel { get; init; }
    }
}
