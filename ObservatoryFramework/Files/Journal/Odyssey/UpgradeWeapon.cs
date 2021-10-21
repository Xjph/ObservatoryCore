namespace Observatory.Framework.Files.Journal
{
    public class UpgradeWeapon : JournalBase
    {
        public string Name { get; init; }
        public string Name_Localised { get; init; }
        public ulong SuitModuleID { get; init; }
        public int Class { get; init; }
        public int Cost { get; init; }
    }
}
