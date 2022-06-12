using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    /// <summary>
    /// Event generated when an item in the codex is scanned.
    /// </summary>
    public class CodexEntry : JournalBase
    {
        /// <summary>
        /// Unique ID of the entry.
        /// </summary>
        public ulong EntryID { get; init; }
        /// <summary>
        /// Name of the item scanned.
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Localised name of the item scanned.
        /// </summary>
        public string Name_Localised { get; init; }
        /// <summary>
        /// Codex sub category of the item scanned.
        /// </summary>
        public string SubCategory { get; init; }
        /// <summary>
        /// Localised sub category name of the item scanned.
        /// </summary>
        public string SubCategory_Localised { get; init; }
        /// <summary>
        /// Codex category of the item scanned.
        /// </summary>
        public string Category { get; init; }
        /// <summary>
        /// Localised category name of the item scanned.
        /// </summary>
        public string Category_Localised { get; init; }
        /// <summary>
        /// Codex region the scan occured in.
        /// </summary>
        public string Region { get; init; }
        /// <summary>
        /// Localised name of the region.
        /// </summary>
        public string Region_Localised { get; init; }
        /// <summary>
        /// Name of the system in which the scan occured.
        /// </summary>
        public string System { get; init; }
        /// <summary>
        /// Unique ID of the system in which the scan occured.
        /// </summary>
        public ulong SystemAddress { get; init; }
        /// <summary>
        /// Name of the nearest surface signal.
        /// </summary>
        public string NearestDestination { get; init; }
        /// <summary>
        /// Localised name of hte nearest surface signal.
        /// </summary>
        public string NearestDestination_Localised { get; init; }
        /// <summary>
        /// Indicator that this codex entry hasn't been previously scanned by the CMDR.
        /// </summary>
        public bool IsNewEntry { get; init; }
        /// <summary>
        /// Indicator that htis codex entry has a trait not previously seen by the CMDR.
        /// </summary>
        public bool NewTraitsDiscovered { get; init; }
        /// <summary>
        /// List of trais of the scanned item.
        /// </summary>
        public ImmutableList<string> Traits { get; init; }
        /// <summary>
        /// Value of the codex entry when sold to Universal Cartographics.
        /// </summary>
        public int VoucherAmount { get; init; }
        /// <summary>
        /// Surface latitude where the scan occured.
        /// </summary>
        public float Latitude { get; init; }
        /// <summary>
        ///  Surface longitude where the scan occured.
        /// </summary>
        public float Longitude { get; init; }
        /// <summary>
        /// Body ID of the system body where the scan occured.
        /// </summary>
        public int BodyID { get; init; }
    }
}
