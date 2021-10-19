namespace Observatory.Framework.Files.ParameterTypes
{
    public class Modules
    {
        public string Slot { get; init; }

        public string Item { get; init; }

        public bool On { get; init; }

        public int Priority { get; init; }

        public double Health { get; init; }

        public double? Value { get; init; }

        public int? AmmoInClip { get; init; }

        public int? AmmoInHopper { get; init; }

        public Engineering Engineering { get; init; }
    }
}
