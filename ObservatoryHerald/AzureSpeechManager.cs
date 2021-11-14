using System;
using System.IO;
using System.Text.Json;
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


        private string cacheIndexFile
        {
            get => cacheLocation.FullName + "VoiceIndex.json";
        }

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


            foreach (var voice in voices)
            {
                voiceOptions.Add(
                    $"{voice.Locale} - {voice.LocalName}", 
                    voice);
            }

            return voiceOptions;
        }

        internal string GetAudioFileFromSsml(string ssml, string voice)
        {
            ssml = AddVoiceToSsml(ssml, voice);
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

        private static string AddVoiceToSsml(string ssml, string voiceName)
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
