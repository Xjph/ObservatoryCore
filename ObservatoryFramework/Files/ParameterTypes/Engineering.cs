using System.Collections.Immutable;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class Engineering
    {
        public ulong EngineerID { get; init; }

        public string Engineer { get; init; }

        public ulong BlueprintID { get; init; }

        public string BlueprintName { get; init; }

        public int Level { get; init; }

        public double Quality { get; init; }

        public string ExperimentalEffect { get; init; }

        public ImmutableList<Modifiers> Modifiers { get; init; }
    }
}
