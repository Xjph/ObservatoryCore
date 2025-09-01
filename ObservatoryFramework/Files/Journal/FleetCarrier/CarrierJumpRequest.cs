using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierJumpRequest : JournalBase
    {
        public string Body { get; init; }
        public int BodyID { get; init; }
        public ulong SystemAddress { get; init; }
        public ulong CarrierID { get; init; }
        public CarrierType CarrierType { get; init; }
        public string SystemName { get; init; }
        public ulong SystemID { get; init; }
        public string DepartureTime { get; init; }

        [JsonIgnore]
        public DateTime DepartureTimeDateTime {
            get => ParseDateTime(DepartureTime);
        }
    }
}
