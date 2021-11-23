using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Elite Dangerous backpack.json file. Describes all the items currently carried by the player.
    /// </summary>
    public class BackpackFile : JournalBase
    {
        /// <summary>
        /// List of all items carried.
        /// </summary>
        public ImmutableList<BackpackItem> Items { get; init; }
        /// <summary>
        /// List of all components carried.
        /// </summary>
        public ImmutableList<BackpackItem> Components { get; init; }
        /// <summary>
        /// List of player consumable items carried.
        /// </summary>
        public ImmutableList<BackpackItem> Consumables { get; init; }
        /// <summary>
        /// List of all data currently stored by the player.
        /// </summary>
        public ImmutableList<BackpackItem> Data { get; init; }
    }
}
