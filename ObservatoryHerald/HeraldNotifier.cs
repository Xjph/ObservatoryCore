using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System;

namespace Observatory.Herald
{
    public class HeraldNotifier : IObservatoryNotifier
    {
        public string Name => "Observatory Herald";

        public string ShortName => "Herald";

        public string Version => throw new NotImplementedException();

        public PluginUI PluginUI => new (PluginUI.UIType.None, null);

        public object Settings { get => heraldSettings; set => heraldSettings = (HeraldSettings)value; }

        public void Load(IObservatoryCore observatoryCore)
        {
            var azureManager = new VoiceAzureCacheManager(heraldSettings, observatoryCore.HttpClient);
            heraldSpeech = new HeraldQueue(azureManager);
        }

        public void OnNotificationEvent(NotificationArgs notificationEventArgs)
        {
            heraldSpeech.Enqueue(notificationEventArgs, heraldSettings.SelectedVoice.ToString());
        }

        private HeraldSettings heraldSettings;
        private HeraldQueue heraldSpeech;
    }
}
