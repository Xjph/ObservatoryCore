namespace Observatory.Framework.Files.ParameterTypes
{
    /// <summary>
    /// Genus object used within SAASignalsFound event
    /// </summary>
    public class GenusType
    {
        /// <summary>
        /// Internal (non-localised) name of genus, e.g.: "$Codex_Ent_Stratum_Genus_Name"
        /// </summary>
        public string Genus { get; init; }
        /// <summary>
        /// Name of genus localised for player locale, e.g.: "Stratum"
        /// </summary>
        public string Genus_Localised { get; init; }
    }
}
