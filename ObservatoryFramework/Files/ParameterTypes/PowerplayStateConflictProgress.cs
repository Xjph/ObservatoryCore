using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.Files.ParameterTypes
{
    public class PowerplayStateConflictProgress : JournalBase
    {
        public string Power { get; init; }
        public double ConflictProgress { get; init; }
    }
}
