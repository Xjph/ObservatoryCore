namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Journal event generated when buying system data from the galaxy map while docked.
    /// </summary>
    public class BuyExplorationData : JournalBase
    {
        /// <summary>
        /// Name of the system for which data was purchased.
        /// </summary>
        public string System { get; init; }
        /// <summary>
        /// Amount paid for the data.
        /// </summary>
        public int Cost { get; init; }
    }
}
