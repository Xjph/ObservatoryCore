using Observatory.Framework;

namespace Observatory.Herald
{
    public class HeraldSettings
    {
        [SettingDisplayName("API Key Override: ")]
        public string AzureAPIKeyOverride { get; set; }

        [SettingDisplayName("Voice")]
        [SettingBackingValue("SelectedVoice")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Dictionary<string, object> Voices {get; internal set;}

        [SettingIgnore]
        public string SelectedVoice { get; set; }

        [SettingBackingValue("SelectedRate")]
        public Dictionary<string, object> Rate
        { get => new Dictionary<string, object> 
            {
                {"Slowest", "0.5"},
                {"Slower", "0.75"},
                {"Default", "1.0"},
                {"Faster", "1.25"},
                {"Fastest", "1.5"}
            }; 
        }

        [SettingIgnore]
        public string SelectedRate { get; set; }

        [SettingDisplayName("Volume")]
        [SettingNumericUseSlider, SettingNumericBounds(0,100,1)]
        public int Volume { get; set;}

        [System.Text.Json.Serialization.JsonIgnore]
        public Action Test { get; internal set; }

        [SettingDisplayName("Enabled")]
        public bool Enabled { get; set; }

        [SettingIgnore]
        public string ApiEndpoint { get; set; }

        [SettingDisplayName("Cache Size (MB): ")]
        public int CacheSize { get; set; }
    }
}
