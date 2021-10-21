using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Framework.Files.Journal
{
    public class Disembark : JournalBase
    {
        public bool SRV { get; init; }
        public bool Taxi { get; init; }
        public bool Multicrew { get; init; }
        public ulong ID { get; init; }
        public string StarSystem { get; init; }
        public ulong SystemAddress { get; init; }
        public string Body { get; init; }
        public int BodyID { get; init; }
        public bool OnStation { get; init; }
        public bool OnPlanet { get; init; }
        public string StationName { get; init; }
        public string StationType { get; init; }
        public ulong MarketID { get; init; }
    }
}
