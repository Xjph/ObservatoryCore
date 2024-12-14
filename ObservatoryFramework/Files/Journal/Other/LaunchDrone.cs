using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class LaunchDrone : JournalBase
    {
        public LimpetDrone Type { get; init; }
    }
}
