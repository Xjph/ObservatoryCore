using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class PowerplayVoucher : PowerplayJoin
    {
        public ImmutableList<string> Systems { get; init; }
    }
}
