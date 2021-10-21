using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class TransferMicroResources : JournalBase
    {
        public ImmutableList<MicroTransfer> Transfers { get; init; }
    }
}
