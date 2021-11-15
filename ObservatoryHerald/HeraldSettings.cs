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
        [SettingDisplayName("API Key Override: ")]
        public string AzureAPIKeyOverride { get; set; }

        [SettingDisplayName("Voice")]
        [SettingBackingValue("SelectedVoice")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Dictionary<string, object> Voices { get; internal set; }

        [SettingIgnore]
        public string SelectedVoice { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Action Test { get; internal set; }

        [SettingDisplayName("Enabled")]
        public bool Enabled { get; set; }
    }
}
