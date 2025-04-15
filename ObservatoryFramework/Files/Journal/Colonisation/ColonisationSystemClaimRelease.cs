using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.Files.Journal.Colonisation
{
    /// <summary>
    /// Written when a colonisation claim is released.
    /// </summary>
    public class ColonisationSystemClaimRelease : JournalBase
    {
        public string StarSystem { get; init; }
        public ulong SystemAddress { get; init; }
    }
}
