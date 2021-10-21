namespace Observatory.Framework.Files.Journal
{
    public class LoadoutEquipModule : JournalBase
    {
        public ulong SuitID { get; init; }
        public string SuitName { get; init; }
        public string SuitName_Localised { get; init; }
        public string SlotName { get; init; }
        public ulong LoadoutID { get; init; }
        public string LoadoutName { get; init; }
        public string ModuleName { get; init; }
        public string ModuleName_Localised { get; init; }
        public ulong SuitModuleID { get; init; }
    }
}
