using Observatory.Framework;

namespace Observatory.Botanist
{
    [SettingSuggestedColumnWidth(450)]
    class BotanistSettings
    {
        [SettingDisplayName("Enable Sampler Status Overlay")]
        public bool OverlayEnabled { get; set; }
        [SettingDisplayName("Status Overlay is sticky until sampling is complete")]
        public bool OverlayIsSticky { get; set; }
    }
}
