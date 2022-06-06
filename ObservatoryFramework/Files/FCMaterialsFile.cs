using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files
{
    /// <summary>
    /// Elite Dangerous fcmaterials.json file. Contains data about current fleet carrier bartender stock.
    /// </summary>
    public class FCMaterialsFile : Journal.JournalBase
    {
        /// <summary>
        /// List of items in stock and in demand from the carrier bartender.
        /// </summary>
        public ImmutableList<FCMaterial> Items { get; init; }
    }
}
