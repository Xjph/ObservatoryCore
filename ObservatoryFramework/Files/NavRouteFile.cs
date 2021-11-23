using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    /// <summary>
    /// Elite Dangerous navroute.json file. Contains data about currently plotted FSD jump route.
    /// </summary>
    public class NavRouteFile : Journal.JournalBase
    {
        /// <summary>
        /// List of star systems and their locations in the current route.
        /// </summary>
        public ImmutableList<Route> Route { get; init; }
    }
}
