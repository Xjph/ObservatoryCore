using Microsoft.CognitiveServices.Speech;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System;

namespace Observatory.Herald
{
    public class HeraldNotifier : IObservatoryNotifier
    {
        public HeraldNotifier()
        {
            heraldSettings = new()
            {
                SelectedVoice = "en-US - Christopher",
                Locale = "en-US",
                AzureAPIKeyOverride = string.Empty,
                Enabled = true
                
            };
        }

        public string Name => "Observatory Herald";

        public string ShortName => "Herald";

        public string Version => typeof(HeraldNotifier).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => new (PluginUI.UIType.None, null);

        public object Settings { get => heraldSettings; set => heraldSettings = (HeraldSettings)value; }

        public void Load(IObservatoryCore observatoryCore)
        {
            var azureManager = new VoiceSpeechManager(heraldSettings, observatoryCore.HttpClient);
            heraldSpeech = new HeraldQueue(azureManager);
            heraldSettings.Test = TestVoice;
        }

        private void TestVoice()
        {
            heraldSpeech.Enqueue(new NotificationArgs() { Title = "Herald voice testing", Detail = "With sample detail text" }, GetAzureNameFromSetting(heraldSettings.SelectedVoice));
        }

        public void OnNotificationEvent(NotificationArgs notificationEventArgs)
        {
            if (heraldSettings.Enabled)
                heraldSpeech.Enqueue(notificationEventArgs, GetAzureNameFromSetting(heraldSettings.SelectedVoice));
        }

        private string GetAzureNameFromSetting(string settingName)
        {
            var voiceInfo = (VoiceInfo)heraldSettings.Voices[settingName];
            return voiceInfo.Name;
        }

        private HeraldSettings heraldSettings;
        private HeraldQueue heraldSpeech;
    }
}
