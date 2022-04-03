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
            // NetCoreAudio is a bit loosey-goosey with its timing,
            // need to add very slight pauses between calls.

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            audioPlayer.SetVolume(volume).Wait();

            string file = speechManager.GetAudioFileFromSsml(ssml, voice, style, rate);

            if (timer.ElapsedMilliseconds < 75)
                Thread.Sleep(75);

            timer.Stop();

            audioPlayer.Play(file).Wait();

            while (audioPlayer.Playing)
                Thread.Sleep(50);

        }
    }
}
