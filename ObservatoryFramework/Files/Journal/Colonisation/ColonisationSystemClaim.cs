using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.Files.Journal.Colonisation
{
    /// <summary>
    /// Written when a system is claimed for colonisation by paying for the claim.
    /// </summary>
    public class ColonisationSystemClaim : JournalBase
    {
        public string StarSystem { get; init; }
        public ulong SystemAddress { get; init; }
    }
}
