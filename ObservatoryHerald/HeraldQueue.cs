using Observatory.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using NetCoreAudio;
using System.Threading;

namespace Observatory.Herald
{
    class HeraldQueue
    {
        private Queue<NotificationArgs> notifications;
        private bool processing;
        private string voice;
        private VoiceSpeechManager azureCacheManager;
        private Player audioPlayer;
        
        public HeraldQueue(VoiceSpeechManager azureCacheManager)
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

            processing = false;
        }

        private void Speak(string text)
        {
            SpeakSsml($"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"\">{text}</voice></speak>");
        }

        private void SpeakSsml(string ssml)
        {
            string file = azureCacheManager.GetAudioFileFromSsml(ssml, voice);
            
            audioPlayer.Play(file);
            while (audioPlayer.Playing)
            {
                Thread.Sleep(20);
            }
        }

        

    }
}
