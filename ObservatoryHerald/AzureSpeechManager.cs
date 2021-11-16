using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml;
using Microsoft.CognitiveServices.Speech;
using System.Collections.ObjectModel;
using Observatory.Framework;

namespace Observatory.Herald
{
    internal class VoiceSpeechManager
    {
        private string azureKey;
        private DirectoryInfo cacheLocation;
        private SpeechConfig speechConfig;
        private SpeechSynthesizer speech;

        internal VoiceSpeechManager(HeraldSettings settings, HttpClient httpClient)
        {
            cacheLocation = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) 
                + $"{Path.DirectorySeparatorChar}ObservatoryCore{Path.DirectorySeparatorChar}ObservatoryHerald{Path.DirectorySeparatorChar}");

            if (!Directory.Exists(cacheLocation.FullName))
            {
                Directory.CreateDirectory(cacheLocation.FullName);
            }
            
            try
            {
                azureKey = GetAzureKey(settings, httpClient);
            }
            catch (Exception ex)
            {
                throw new PluginException("Herald", "Unable to retrieve Azure API key.", ex);
            }

            try
            {
                speechConfig = SpeechConfig.FromSubscription(azureKey, "eastus");
            }
            catch (Exception ex)
            {
                throw new PluginException("Herald", "Error retrieving Azure account details.", ex);
            }

            speech = new(speechConfig, null);

            settings.Voices = PopulateVoiceSettingOptions();
        }

        private Dictionary<string, object> PopulateVoiceSettingOptions()
        {
            ReadOnlyCollection<VoiceInfo> voices;

            try
            {
                voices = speech.GetVoicesAsync().Result.Voices;
            }
            catch (Exception ex)
            {
                throw new PluginException("Herald", "Unable to retrieve voice list from Azure.", ex);
            }
            
            var voiceOptions = new Dictionary<string, object>();

            var englishSpeakingVoices = from v in voices 
                                        where v.Locale.StartsWith("en-") 
                                        select v;

            foreach (var voice in englishSpeakingVoices)
            {
                string demonym = GetDemonymFromLocale(voice.Locale);

                voiceOptions.Add(
                    $"{demonym} - {voice.LocalName}",
                    voice);

                foreach (var style in voice.StyleList)
                {
                    if (!string.IsNullOrWhiteSpace(style))
                        voiceOptions.Add(
                            $"{demonym} - {voice.LocalName} - {style}",
                            voice);
                }
            }

            return voiceOptions;
        }

        private static string GetDemonymFromLocale(string locale)
        {
            string demonym;

            switch (locale)
            {
                case "en-AU":
                    demonym = "Australian";
                    break;
                case "en-CA":
                    demonym = "Canadian";
                    break;
                case "en-GB":
                    demonym = "British";
                    break;
                case "en-HK":
                    demonym = "Hong Konger";
                    break;
                case "en-IE":
                    demonym = "Irish";
                    break;
                case "en-IN":
                    demonym = "Indian";
                    break;
                case "en-KE":
                    demonym = "Kenyan";
                    break;
                case "en-NG":
                    demonym = "Nigerian";
                    break;
                case "en-NZ":
                    demonym = "Kiwi";
                    break;
                case "en-PH":
                    demonym = "Filipino";
                    break;
                case "en-SG":
                    demonym = "Singaporean";
                    break;
                case "en-TZ":
                    demonym = "Tanzanian";
                    break;
                case "en-US":
                    demonym = "American";
                    break;
                case "en-ZA":
                    demonym = "South African";
                    break;
                default:
                    demonym = locale;
                    break;
            }

            return demonym;
        }

        internal string GetAudioFileFromSsml(string ssml, string voice, string style)
        {
            ssml = AddVoiceToSsml(ssml, voice, style);
            string ssmlHash = FNV64(ssml).ToString("X");

            string audioFile = cacheLocation + ssmlHash + ".wav";

            if (!File.Exists(audioFile))
            {
                using var stream = RequestFromAzure(ssml);
                stream.SaveToWaveFileAsync(audioFile).Wait();
            }

            return audioFile;
        }

        private static ulong FNV64(string data)
        {
            string lower_data = data.ToLower();
            ulong hash = 0xcbf29ce484222325uL;
            for (int i = 0; i < lower_data.Length; i++)
            {
                byte b = (byte)lower_data[i];
                hash *= 1099511628211uL;
                hash ^= b;
            }
            return hash;
        }

        private AudioDataStream RequestFromAzure(string ssml)
        {
            try
            {
                var result = speech.SpeakSsmlAsync(ssml).Result;
                return AudioDataStream.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new PluginException("Herald", "Unable to retrieve audio from Azure.", ex);
            }
        }

        private static string AddVoiceToSsml(string ssml, string voiceName, string styleName)
        {
            XmlDocument ssmlDoc = new();
            ssmlDoc.LoadXml(ssml);

            var ssmlNamespace = ssmlDoc.DocumentElement.NamespaceURI;
            XmlNamespaceManager ssmlNs = new(ssmlDoc.NameTable);
            ssmlNs.AddNamespace("ssml", ssmlNamespace);


            var voiceNode = ssmlDoc.SelectSingleNode("/ssml:speak/ssml:voice", ssmlNs);

            voiceNode.Attributes.GetNamedItem("name").Value = voiceName;

            string ssmlResult;

            if (!string.IsNullOrWhiteSpace(styleName))
            {
                voiceNode.InnerText = $"<mstts:express-as style=\"{styleName}\">" + voiceNode.InnerText + "</mstts:express-as>";

                // This is a kludge but I don't feel like dealing with System.Xml and namespaces
                ssmlResult = ssmlDoc.OuterXml
                    .Replace(" xmlns=", " xmlns:mstts=\"https://www.w3.org/2001/mstts\" xmlns=")
                    .Replace($"&lt;mstts:express-as style=\"{styleName}\"&gt;", $"<mstts:express-as style=\"{styleName}\">")
                    .Replace("&lt;/mstts:express-as&gt;", "</mstts:express-as>");
            }
            else
            {
                ssmlResult = ssmlDoc.OuterXml;
            }

            return ssmlResult;
        }

        private static string GetAzureKey(HeraldSettings settings, HttpClient httpClient)
        {
            string azureKey;

            if (string.IsNullOrWhiteSpace(settings.AzureAPIKeyOverride))
            {
                azureKey = httpClient.GetStringAsync("https://xjph.net/Observatory/ObservatoryHeraldAzureKey").Result;
            }
            else
            {
                azureKey = settings.AzureAPIKeyOverride;
            }

            return azureKey;
        }
    }
}
