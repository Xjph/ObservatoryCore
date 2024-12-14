using System.Text.Json.Serialization;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class MicroResource
    {
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public MicroCategory Category { get; init; }
        public int Count { get; init; }
    }
}
