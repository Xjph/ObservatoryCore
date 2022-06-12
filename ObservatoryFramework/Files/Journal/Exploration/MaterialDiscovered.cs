namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated the first time a CMDR finds a particular material resource.
    /// </summary>
    public class MaterialDiscovered : JournalBase
    {
        /// <summary>
        /// Category of the material.
        /// </summary>
        public string Category { get; init; }
        /// <summary>
        /// Name of the material.
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Localised name of the material.
        /// </summary>
        public string Name_Localised { get; init; }
        /// <summary>
        /// Count of the number of discovered items in that category by the CMDR.
        /// </summary>
        public int DiscoveryNumber { get; init; }
    }
}
