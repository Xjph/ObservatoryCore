using System.Collections.Immutable;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CargoTransfer : JournalBase
    {
        public ImmutableList<CargoTransferDetail> Transfers { get; init; }
    }
}
