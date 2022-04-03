﻿using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System.Text.Json;

namespace Observatory.Herald
{
    public class HeraldNotifier : IObservatoryNotifier
    {
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
                Volume = 75,
                Enabled = false,
                ApiEndpoint = "https://api.observatory.xjph.net/AzureVoice",
                CacheSize = 100
            };
        }

        public string Name => "Observatory Herald";

        public string ShortName => "Herald";

        public string Version => typeof(HeraldNotifier).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => new (PluginUI.UIType.None, null);

        public object Settings
        {
            get => heraldSettings;
            set
            {
                // Need to perform validation here, older
                // version settings object not fully compatible.
                var savedSettings = (HeraldSettings)value;
                if (string.IsNullOrWhiteSpace(savedSettings.SelectedVoice))
                    savedSettings.SelectedVoice = heraldSettings.SelectedVoice;

                if (string.IsNullOrWhiteSpace(savedSettings.SelectedRate))
                    savedSettings.SelectedRate = heraldSettings.SelectedRate;

                if (string.IsNullOrWhiteSpace(savedSettings.ApiEndpoint))
                    savedSettings.ApiEndpoint = heraldSettings.ApiEndpoint;

                heraldSettings = savedSettings;
            }
        }
        public void Load(IObservatoryCore observatoryCore)
        {
            var speechManager = new SpeechRequestManager(heraldSettings, observatoryCore.HttpClient, observatoryCore.PluginStorageFolder);
            heraldSpeech = new HeraldQueue(speechManager);
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
                heraldSettings.Rate[heraldSettings.SelectedRate].ToString(),
                heraldSettings.Volume);
        }

        public void OnNotificationEvent(NotificationArgs notificationEventArgs)
        {
            if (heraldSettings.Enabled)
                heraldSpeech.Enqueue(
                    notificationEventArgs, 
                    GetAzureNameFromSetting(heraldSettings.SelectedVoice),
                    GetAzureStyleNameFromSetting(heraldSettings.SelectedVoice),
                    heraldSettings.Rate[heraldSettings.SelectedRate].ToString(), 
                    heraldSettings.Volume);
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
