using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.Files.Journal.Colonisation
{
    /// <summary>
    /// Written every 15 seconds while docked at a construction depot.
    /// </summary>
    public class ColonisationConstructionDepot : JournalBase
    {
        public ulong MarketID { get; init; }

        public float ConstructionProgress { get; init; }

        public bool ConstructionComplete { get; init; }

        public bool ConstructionFailed { get; init; }

        public ImmutableList<RequiredResource> ResourcesRequired { get; init; }


    }
}
