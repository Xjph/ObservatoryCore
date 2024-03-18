using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
