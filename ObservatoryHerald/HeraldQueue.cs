using Observatory.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using NetCoreAudio;

namespace Observatory.Herald
{
    class HeraldQueue
    {
        private Queue<NotificationArgs> notifications;
        private bool processing;
        private string voice;
        private VoiceAzureCacheManager azureCacheManager;
        private Player audioPlayer;
        
        public HeraldQueue(VoiceAzureCacheManager azureCacheManager)
        {
            this.azureCacheManager = azureCacheManager;
            processing = false;
            notifications = new();
            audioPlayer = new();
        }


        internal void Enqueue(NotificationArgs notification, string selectedVoice)
        {
            voice = selectedVoice;
            notifications.Enqueue(notification);

            if (!processing)
            {
                processing = true;
                ProcessQueueAsync();
            }
        }

        private async void ProcessQueueAsync()
        {
            await Task.Factory.StartNew(ProcessQueue);
        }

        private void ProcessQueue()
        {
            while (notifications.Any())
            {
                var notification = notifications.Dequeue();

                if (string.IsNullOrWhiteSpace(notification.TitleSsml))
                {
                    Speak(notification.Title);
                }
                else
                {
                    SpeakSsml(notification.TitleSsml);
                }
                
                if (string.IsNullOrWhiteSpace(notification.DetailSsml))
                {
                    Speak(notification.Detail);
                }
                else
                {
                    SpeakSsml(notification.DetailSsml);
                }

            }
        }

        private void Speak(string text)
        {
            SpeakSsml($"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"\">{text}</voice></speak>");
        }

        private void SpeakSsml(string ssml)
        {
            string file = azureCacheManager.GetAudioFileFromSsml(ssml, voice);

            audioPlayer.Play(file).Wait();
        }

        

    }
}
