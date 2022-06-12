namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated when a material resource is collected.
    /// </summary>
    public class MaterialCollected : JournalBase
    {
        /// <summary>
        /// Category to which the material belongs.
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
        /// Count of the material.
        /// </summary>
        public int Count { get; init; }
    }
}
