using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Botanist
{
    class BotanistSettings
    {
        [SettingDisplayName("Enable Sampler Status Overlay")]
        public bool OverlayEnabled { get; set; }
    }
}
