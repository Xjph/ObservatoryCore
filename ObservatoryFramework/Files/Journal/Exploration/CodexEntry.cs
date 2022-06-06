using System.Collections.Immutable;

namespace Observatory.Framework.Files.Journal
{
    public class CodexEntry : JournalBase
    {
        public ulong EntryID { get; init; }
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public string SubCategory { get; init; }
        public string SubCategory_Localised { get; init; }
        public string Category { get; init; }
        public string Category_Localised { get; init; }
        public string Region { get; init; }
        public string Region_Localised { get; init; }
        public string System { get; init; }
        public ulong SystemAddress { get; init; }
        public string NearestDestination { get; init; }
        public string NearestDestination_Localised { get; init; }
        public bool IsNewEntry { get; init; }
        public bool NewTraitsDiscovered { get; init; }
        public ImmutableList<string> Traits { get; init; }
        public int VoucherAmount { get; init; }
        public float Latitude { get; init; }
        public float Longitude { get; init; }
        public int BodyID { get; init; }
    }
}
