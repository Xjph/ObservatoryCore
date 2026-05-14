using Observatory.Framework;

namespace Observatory.Communicator
{
    internal class CommunicatorSettings
    {
        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Private")]
        public bool NotifyPrivate { get; set; } = true;

        [SettingDisplayName("Wing")]
        public bool NotifyWing { get; set; } = true;

        [SettingDisplayName("Squadron")]
        public bool NotifySquadron { get; set; } = true;

        [SettingDisplayName("System")]
        public bool NotifySystem { get; set; } = true;

        [SettingDisplayName("Local")]
        public bool NotifyLocal { get; set; } = true;

        [SettingDisplayName("NPC")]
        public bool NotifyNPC { get; set; } = false;

        [SettingDisplayName("Suppress Voice")]
        public bool SuppressVoice { get; set; } = false;

        [SettingIgnore]
        public bool ShowPrivate { get; set; } = true;

        [SettingIgnore]
        public bool ShowWing { get; set; } = true;

        [SettingIgnore]
        public bool ShowSquadron { get; set; } = true;

        [SettingIgnore]
        public bool ShowSystem { get; set; } = true;

        [SettingIgnore]
        public bool ShowLocal { get; set; } = true;

        [SettingIgnore]
        public bool ShowNPC { get; set; } = false;
    }
}
