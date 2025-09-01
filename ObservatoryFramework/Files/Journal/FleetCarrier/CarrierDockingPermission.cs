using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class CarrierDockingPermission : JournalBase
    {
        public ulong CarrierID { get; init; }
        public CarrierType CarrierType { get; init; }
        public CarrierDockingAccess DockingAccess { get; init; }
        public bool AllowNotorious { get; init; }
    }
}
