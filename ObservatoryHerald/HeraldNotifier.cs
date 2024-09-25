using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System.Text.Json;

namespace Observatory.Herald
{
    public class HeraldNotifier : IObservatoryNotifier
    {
        private IObservatoryCore Core;
        private AboutInfo _aboutInfo = new()
        {
            FullName = "Observatory Herald",
            ShortName = "Herald",
            Description = "Herald is a core plugin for Observatory, designed to provide cloud-based higher-quality speech synthesis.",
            AuthorName = "Vithigar",
            Links = new()
            {
                new AboutLink("github", "https://github.com/Xjph/ObservatoryCore"),
                new AboutLink("Documentation", "https://observatory.xjph.net/usage/plugins/herald"),
            }
        };

        public HeraldNotifier()
        {
            heraldSettings = DefaultSettings;
        }

        private static HeraldSettings DefaultSettings
        {
            get => new HeraldSettings()
            {
                SelectedVoice = "American - Christopher",
                SelectedRate = "Default",
                ApiEndpoint = "https://api.observatory.xjph.net/AzureVoice",
                CacheSize = 100
            };
        }

        public AboutInfo AboutInfo => _aboutInfo;

        public bool OverrideAudioNotifications => true;

        public string Version => typeof(HeraldNotifier).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => new (PluginUI.UIType.None, null);

        public object Settings
        {
            get => heraldSettings;
            set
            {
                // Need to perform migration here, older
                // version settings object not fully compatible.
                var savedSettings = (HeraldSettings)value;
                if (string.IsNullOrWhiteSpace(savedSettings.SelectedRate))
                {
                    heraldSettings.SelectedVoice = savedSettings.SelectedVoice;
                }
                else
                {
                    heraldSettings = savedSettings;
                }
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            Core = observatoryCore;
            var speechManager = new SpeechRequestManager(
                heraldSettings, observatoryCore.HttpClient, observatoryCore.PluginStorageFolder, observatoryCore.GetPluginErrorLogger(this));
            heraldSpeech = new HeraldQueue(speechManager, observatoryCore.GetPluginErrorLogger(this), observatoryCore);
            heraldSettings.Test = TestVoice;
        }

        private void TestVoice()
        {
            heraldSpeech.Enqueue(
                new NotificationArgs() 
                { 
                    Title = "Herald voice testing", 
                    Detail = $"This is {heraldSettings.SelectedVoice.Split(" - ")[1]}." 
                }, 
                GetAzureNameFromSetting(heraldSettings.SelectedVoice),
                GetAzureStyleNameFromSetting(heraldSettings.SelectedVoice),
                heraldSettings.Rate[heraldSettings.SelectedRate].ToString());
        }

        public void OnNotificationEvent(NotificationArgs notificationEventArgs)
        {
            if (Core.IsLogMonitorBatchReading) return;

            if (notificationEventArgs.Rendering.HasFlag(NotificationRendering.NativeVocal))
                heraldSpeech.Enqueue(
                    notificationEventArgs, 
                    GetAzureNameFromSetting(heraldSettings.SelectedVoice),
                    GetAzureStyleNameFromSetting(heraldSettings.SelectedVoice),
                    heraldSettings.Rate[heraldSettings.SelectedRate].ToString());
        }

        private string GetAzureNameFromSetting(string settingName)
        {
            var voiceInfo = (JsonElement)heraldSettings.Voices[settingName];
            return voiceInfo.GetProperty("ShortName").GetString();
        }

        private string GetAzureStyleNameFromSetting(string settingName)
        {
            string[] settingParts = settingName.Split(" - ");
            
            if (settingParts.Length == 3)
                return settingParts[2];
            else
                return string.Empty;
        }

        private HeraldSettings heraldSettings;
        private HeraldQueue heraldSpeech;
    }
}
