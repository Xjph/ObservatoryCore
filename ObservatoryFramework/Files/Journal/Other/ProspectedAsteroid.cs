using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class ProspectedAsteroid : JournalBase
    {
        public ImmutableList<ProspectMaterial> Materials { get; init; }
        public string Content { get; init; }
        public string Content_Localised { get; init; }
        public string MotherlodeMaterial { get; init; }
        public string MotherlodeMaterial_Localised { get; init; }
        public float Remaining { get; init; }
    }
}
