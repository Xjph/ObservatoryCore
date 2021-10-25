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
            await Task.Factory.StartNew(ProcessQueue);
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

            //If the SSML already has a voice element leave it alone.
            if (ssmlDoc.SelectSingleNode("/ssml:speak/ssml:voice", ssmlNs) == null)
            {
                //Preserve existing content to place it in new voice element
                string speakContent = ssmlDoc.DocumentElement.InnerXml;
                speakContent = speakContent.Replace($"xmlns=\"{ssmlNs.LookupNamespace("ssml")}\"", string.Empty);

                //Crete new voice element and name attribute objects
                var voiceElement = ssmlDoc.CreateElement("voice", ssmlNs.LookupNamespace("ssml"));
                var voiceAttribute = ssmlDoc.CreateAttribute("name");

                //Update content of new element
                voiceAttribute.Value = voiceName;
                voiceElement.Attributes.Append(voiceAttribute);
                voiceElement.InnerXml = speakContent;
                
                //Clear existing content and insert new element
                ssmlDoc.DocumentElement.InnerText = string.Empty;
                ssmlDoc.DocumentElement.AppendChild(voiceElement);

                ssml = ssmlDoc.OuterXml;
            }

            //If I leave the namespace in speakContent above it's left behind as a redundant
            //attribute which breaks the speech generation.
            //If I remove it then the XmlDoc explicitly adds an empty namespace which *also*
            //breaks speech generation.
            //The empty one is easier to remove later, so that's what I'm doing, but if someone
            //has a better suggestion I'm all for it.
            return ssml.Replace("xmlns=\"\"", string.Empty);
        }
    }
}
