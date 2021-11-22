using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierCrewServices : JournalBase
    {
        public ulong CarrierID { get; init; }
        public string CrewRole { get; init; }
        public string CrewName { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CarrierCrewOperation Operation { get; init; }
    }
}
