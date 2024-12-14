using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class DockingDenied : DockingCancelled
    {
        public Reason Reason { get; init; }
    }
}
