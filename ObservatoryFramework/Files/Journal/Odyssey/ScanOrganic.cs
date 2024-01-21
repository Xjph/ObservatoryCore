using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.Journal
{
    public class ScanOrganic : JournalBase
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ScanOrganicType ScanType { get; init; }
        public string Genus { get; init; }
        public string Genus_Localised { get; init; }
        public string Species { get; init; }
        public string Species_Localised { get; init; }
        public string Variant {  get; init; }
        public string Variant_Localised { get; init; }
        public ulong SystemAddress { get; init; }
        public int Body { get; init; }
    }
}
