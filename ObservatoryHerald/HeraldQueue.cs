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
        private string style;
        private string rate;
        private byte volume;
        private SpeechRequestManager speechManager;
        private Player audioPlayer;
        
        public HeraldQueue(SpeechRequestManager speechManager)
        {
            this.speechManager = speechManager;
            processing = false;
            notifications = new();
            audioPlayer = new();
        }


        internal void Enqueue(NotificationArgs notification, string selectedVoice, string selectedStyle = "", string selectedRate = "", int volume = 75)
        {
            voice = selectedVoice;
            style = selectedStyle;
            rate = selectedRate;
            // Ignore invalid values; assume default.
            volume = volume >= 0 && volume <= 100 ? volume : 75;

            // Volume is perceived logarithmically, convert to exponential curve
            // to make perceived volume more in line with value set.
            this.volume = ((byte)System.Math.Floor(System.Math.Pow(volume / 100.0, 2.0) * 100));

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
                audioPlayer.SetVolume(volume).Wait();
                var notification = notifications.Dequeue();

                Task<string>[] audioRequestTasks = new Task<string> [2];
                

                if (string.IsNullOrWhiteSpace(notification.TitleSsml))
                {
                    audioRequestTasks[0] = RetrieveAudioToFile(notification.Title);
                }
                else
                {
                    audioRequestTasks[0] = RetrieveAudioSsmlToFile(notification.TitleSsml);
                }

                if (string.IsNullOrWhiteSpace(notification.DetailSsml))
                {
                    audioRequestTasks[1] = RetrieveAudioToFile(notification.Detail);
                }
                else
                {
                    audioRequestTasks[1] = RetrieveAudioSsmlToFile(notification.DetailSsml);
                }

                PlayAudioRequestsSequentially(audioRequestTasks);
            }

            processing = false;
        }

        private async Task<string> RetrieveAudioToFile(string text)
        {
            return await RetrieveAudioSsmlToFile($"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"\">{text}</voice></speak>");
        }

        private async Task<string> RetrieveAudioSsmlToFile(string ssml)
        {
            return await speechManager.GetAudioFileFromSsml(ssml, voice, style, rate);
        }

        private void PlayAudioRequestsSequentially(Task<string>[] requestTasks)
        {
            foreach (var request in requestTasks)
            {
                string file = request.Result;
                audioPlayer.Play(file).Wait();

                while (audioPlayer.Playing)
                    Thread.Sleep(50);

            }
        }
    }
}
