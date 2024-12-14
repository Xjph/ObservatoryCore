using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class VehicleSwitch : JournalBase
    {
        public VehicleSwitchTo To { get; init; }
    }
}
