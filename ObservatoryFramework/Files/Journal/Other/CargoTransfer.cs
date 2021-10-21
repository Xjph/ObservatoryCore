using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class CargoTransfer : JournalBase
    {
        public ImmutableList<CargoTransferDetail> Transfers { get; init; }
    }
}
