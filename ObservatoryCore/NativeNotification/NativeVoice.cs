using Observatory.Framework;
using System.Collections.Generic;
using System.Xml;
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
            try
            {
                await Task.Factory.StartNew(ProcessQueue);
            }
            catch (System.Exception ex)
            {
                ObservatoryCore.LogError(ex, " - Native Voice Notifier");
            }
        }

        private void ProcessQueue()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string voice = Properties.Core.Default.VoiceSelected;

                var speech = new SpeechSynthesizer()
                {
                    Volume = Properties.Core.Default.VoiceVolume,
                    Rate = Properties.Core.Default.VoiceRate
                };
                speech.SelectVoice(voice);

                while (notificationEvents.Any())
                {
                    var notification = notificationEvents.Dequeue();

                    if (notification.TitleSsml?.Length > 0)
                    {
                        string ssml = AddVoiceToSsml(notification.TitleSsml, voice);
                        speech.SpeakSsml(ssml);
                    }
                    else
                    {
                        speech.Speak(notification.Title);
                    }

                    if (notification.DetailSsml?.Length > 0)
                    {
                        string ssml = AddVoiceToSsml(notification.DetailSsml, voice);
                        speech.SpeakSsml(ssml);
                    }
                    else
                    {
                        speech.Speak(notification.Detail);
                    }
                }
            }
            processing = false;
        }

        private string AddVoiceToSsml(string ssml, string voiceName)
        {
            XmlDocument ssmlDoc = new();
            ssmlDoc.LoadXml(ssml);

            var ssmlNamespace = ssmlDoc.DocumentElement.NamespaceURI;
            XmlNamespaceManager ssmlNs = new(ssmlDoc.NameTable);
            ssmlNs.AddNamespace("ssml", ssmlNamespace);


            var voiceNode = ssmlDoc.SelectSingleNode("/ssml:speak/ssml:voice", ssmlNs);

            voiceNode.Attributes.GetNamedItem("name").Value = voiceName;

            return ssmlDoc.OuterXml;
        }
    }
}
