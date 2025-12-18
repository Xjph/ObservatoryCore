using Observatory.Framework;
using System.Xml;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Observatory.Utils;
using Observatory.Assets;

namespace Observatory.NativeNotification
{
    internal class NativeVoice
    {
        private readonly Queue<NotificationArgs> notificationEvents;
        private readonly AudioHandler audioHandler;

        public NativeVoice(AudioHandler audiohandler)
        {
            notificationEvents = new();
            audioHandler = audiohandler;
        }

        public void AudioHandlerEnqueue(NotificationArgs eventArgs)
        {
            try
            {
                string filename;

#if !PROTON
                if (Properties.Core.Default.ChimeEnabled)
#endif
                {
                    filename = Path.GetTempPath() + "ObsCore_" + Guid.NewGuid().ToString() + ".wav";
                    using UnmanagedMemoryStream ms = Properties.Core.Default.ChimeSelected switch
                    {
                        1 => Resources.ObservatoryNotification1,
                        2 => Resources.ObservatoryNotification2,
                        3 => Resources.ObservatoryNotification3,
                        4 => Resources.ObservatoryNotification4,
                        5 => Resources.ObservatoryNotification5,
                        _ => Resources.ObservatoryNotification0,
                    };
                    byte[] wavData = new byte[ms.Length];
                    ms.Read(wavData, 0, wavData.Length);
                    ms.Close();
                    File.WriteAllBytes(filename, wavData);
                }
#if !PROTON
                else
                {
                    string voice = Properties.Core.Default.VoiceSelected;

                    var speech = new SpeechSynthesizer()
                    {
                        Volume = Properties.Core.Default.VoiceVolume,
                        Rate = Properties.Core.Default.VoiceRate
                    };
                    speech.InjectOneCoreVoices();
                    speech.SelectVoice(voice);
                    filename = Path.GetTempPath() + "ObsCore_" + Guid.NewGuid().ToString() + ".wav";
                    speech.SetOutputToWaveFile(filename);
                    var notification = eventArgs;

                    if (!audioHandler.HasMatching(notification))
                    {
                        if (notification.TitleSsml?.Length > 0)
                        {
                            string ssml = AddVoiceToSsml(notification.TitleSsml, voice);
                            speech.SpeakSsml(ssml);
                        }
                        else
                        {
                            speech.Speak(notification.Title);
                        }
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
                }
#endif
                
                audioHandler.EnqueueAndPlay(filename, new() { DeleteAfterPlay = true }, eventArgs);
                
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
