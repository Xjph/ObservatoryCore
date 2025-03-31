using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.Files.Journal
{
    public class PowerplayMerits : JournalBase
    {
        public string Power { get; init; }
        public int MeritsGained { get; init; }
        public int TotalMerits { get; init; }
    }
}
