using System.Text.Json.Serialization;
using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class Synthesis : JournalBase
    {
        public string Name { get; init; }

        [JsonConverter(typeof(MaterialConverter))]
        public ImmutableList<Material> Materials { get; init; }
    }
}
