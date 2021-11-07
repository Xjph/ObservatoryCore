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
    internal class VoiceAzureCacheManager
    {
        private string azureKey;
        private DirectoryInfo cacheLocation;
        private Dictionary<string, string> cacheIndex;
        private SpeechConfig speechConfig;
        private SpeechSynthesizer speech;


        private string cacheIndexFile
        {
            get => cacheLocation.FullName + "\\VoiceIndex.json";
        }

        internal VoiceAzureCacheManager(HeraldSettings settings, HttpClient httpClient)
        {
            cacheLocation = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ObservatoryCore\\ObservatoryHerald\\");

            if (!Directory.Exists(cacheLocation.FullName))
            {
                Directory.CreateDirectory(cacheLocation.FullName);
            }

            if (File.Exists(cacheIndexFile))
            {
                try
                {
                    cacheIndex = JsonSerializer.Deserialize<Dictionary<string, string>>(cacheIndexFile);
                }
                catch
                {
                    cacheIndex = new();
                }
            }
            else
            {
                cacheIndex = new();
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
        }

        internal string GetAudioFileFromSsml(string ssml, string voice)
        {
            ssml = AddVoiceToSsml(ssml, voice);
            string ssmlHash = ssml.GetHashCode().ToString("X");

            string audioFile;

            if (cacheIndex.ContainsKey(ssmlHash))
            {
                audioFile = cacheIndex[ssmlHash];
            }
            else
            {
                using var stream = RequestFromAzure(ssml);
                audioFile = CommitToCache(ssmlHash, stream);
            }

            return audioFile;
        }

        private string CommitToCache(string ssmlHash, AudioDataStream audioData)
        {
            string newFile = Guid.NewGuid().ToString("D");
            audioData.SaveToWaveFileAsync(cacheLocation + Path.DirectorySeparatorChar.ToString() + newFile + ".wav").Wait();
            cacheIndex.Add(ssmlHash, newFile);
            return newFile;
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

        internal ReadOnlyCollection<VoiceInfo> GetVoices()
        {
            try
            {
                return speech.GetVoicesAsync().Result.Voices;
            }
            catch (Exception ex)
            {
                throw new PluginException("Herald", "Unable to retrieve voice list from Azure.", ex);
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
