using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Herald
{
    public class HeraldSettings
    {
        [SettingDisplayName("Azure API Key Override")]
        public string AzureAPIKeyOverride { get; set; }

        [SettingDisplayName("Voice")]
        [SettingBackingValue("SelectedVoice")]
        public Dictionary<string, object> Voices { get; }

        [SettingIgnore]
        public object SelectedVoice { get; set; }
    }
}
