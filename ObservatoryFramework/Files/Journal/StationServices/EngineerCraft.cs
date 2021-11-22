using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json.Serialization;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class EngineerCraft : JournalBase
    {
        public string Engineer { get; init; }
        public ulong EngineerID { get; init; }
        public string Blueprint { get; init; }
        public string Slot { get; init; }
        public string Module { get; init; }
        public ulong BlueprintID { get; init; }
        public string BlueprintName { get; init; }
        public string ExperimentalEffect { get; init; }
        public string ExperimentalEffect_Localised { get; init; }
        public int Level { get; init; }
        public float Quality { get; init; }
        public string ApplyExperimentalEffect { get; init; }
        [JsonConverter(typeof(MaterialConverter))]
        public ImmutableList<Material> Ingredients { get; init; }
        public ImmutableList<Modifier> Modifiers { get; init; }
    }
}
