#if PROTON

using Observatory.Framework;

namespace Observatory.NativeNotification
{
    public class NativeVoice
    {
        public NativeVoice(Object _) { }

        public void AudioHandlerEnqueue(NotificationArgs eventArgs)
        {
            // stub
        }

        
    }
}

#else

using Observatory.Framework;
using System.Xml;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Observatory.Utils;

namespace Observatory.NativeNotification
{
    internal class NativeVoice
    {
        private readonly Queue<NotificationArgs> notificationEvents;
        private bool processing;
        private readonly AudioHandler audioHandler;

        public NativeVoice(AudioHandler audiohandler)
        {
            notificationEvents = new();
            processing = false;
            audioHandler = audiohandler;
        }

        public void AudioHandlerEnqueue(NotificationArgs eventArgs)
        {
            try
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
                    string filename = Path.GetTempPath() + "ObsCore_" + Guid.NewGuid().ToString() + ".wav";
                    speech.SetOutputToWaveFile(filename);
                    var notification = eventArgs;

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
                    speech.Dispose();
                    audioHandler.EnqueueAndPlay(filename, new() { DeleteAfterPlay = true });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static string AddVoiceToSsml(string ssml, string voiceName)
        {
            XmlDocument ssmlDoc = new();
            ssmlDoc.LoadXml(ssml);

            var ssmlNamespace = ssmlDoc.DocumentElement?.NamespaceURI;
            XmlNamespaceManager ssmlNs = new(ssmlDoc.NameTable);
            ssmlNs.AddNamespace("ssml", ssmlNamespace ?? string.Empty);


            var voiceNode = ssmlDoc.SelectSingleNode("/ssml:speak/ssml:voice", ssmlNs);

            var voiceNameNode = voiceNode?.Attributes?.GetNamedItem("name");
            if (voiceNameNode != null)
            {
                voiceNameNode.Value = voiceName;
            }
            
            return ssmlDoc.OuterXml;
        }
    }
}

#endif