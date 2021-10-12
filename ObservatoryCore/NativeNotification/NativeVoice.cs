using Observatory.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;

namespace Observatory.NativeNotification
{
    public class NativeVoice
    {
        private Queue<NotificationArgs> notificationEvents;
        private bool processing;

        public NativeVoice()
        {
            notificationEvents = new();
            processing = false;
        }

        public void EnqueueAndAnnounce(NotificationArgs eventArgs)
        {
            notificationEvents.Enqueue(eventArgs);
            
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var speech = new SpeechSynthesizer()
                {
                    Volume = Properties.Core.Default.VoiceVolume,
                    Rate = Properties.Core.Default.VoiceRate
                };
                speech.SelectVoice(Properties.Core.Default.VoiceSelected);

                while (notificationEvents.Any())
                {
                    var notification = notificationEvents.Dequeue();

                    if (notification.TitleSsml?.Length > 0)
                    {
                        speech.SpeakSsml(notification.TitleSsml);
                    }
                    else
                    {
                        speech.Speak(notification.Title);
                    }

                    if (notification.DetailSsml?.Length > 0)
                    {
                        speech.SpeakSsml(notification.DetailSsml);
                    }
                    else
                    {
                        speech.Speak(notification.Detail);
                    }
                }
            }
            processing = false;
        }
    }
}
