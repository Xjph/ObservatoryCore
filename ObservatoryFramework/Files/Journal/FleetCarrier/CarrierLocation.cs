using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierLocation : JournalBase
    {
        public ulong CarrierID { get; init; }
        public string StarSystem { get; init; }
        public ulong SystemAddress { get; init; }
        public int BodyID { get; init; }
    }
}
