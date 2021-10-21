using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    public class NavRouteFile : Journal.JournalBase
    {
        public ImmutableList<Route> Route { get; init; }
    }
}
