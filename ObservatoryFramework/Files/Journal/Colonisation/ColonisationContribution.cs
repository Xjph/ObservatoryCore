using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal.Colonisation
{
    /// <summary>
    /// Written when contributing resources to a colonisation effort
    /// </summary>
    public class ColonisationContribution : JournalBase
    {
        public ulong MarketID { get; init; }

        public ImmutableList<ContributedResource> Contributions { get; init; }
    }
}
