using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Observatory.Framework.Files.Journal;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class PowerplayStateConflictProgress : JournalBase
    {
        public string Power { get; init; }
        public double ConflictProgress { get; init; }
    }
}
