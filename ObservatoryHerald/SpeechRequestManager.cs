using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Security.Cryptography;

namespace Observatory.Herald
{
    class SpeechRequestManager
    {
        private HttpClient httpClient;
        private string ApiKey;
        private string ApiEndpoint;
        private DirectoryInfo cacheLocation;
        private int cacheSize;
        
        internal SpeechRequestManager(HeraldSettings settings, HttpClient httpClient)
        {
            ApiKey = ObservatoryAPI.ApiKey;
            ApiEndpoint = settings.ApiEndpoint;
            this.httpClient = httpClient;
            cacheSize = settings.CacheSize;
            cacheLocation = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + $"{Path.DirectorySeparatorChar}ObservatoryCore{Path.DirectorySeparatorChar}ObservatoryHerald{Path.DirectorySeparatorChar}");
            
            if (!Directory.Exists(cacheLocation.FullName))
            {
                Directory.CreateDirectory(cacheLocation.FullName);
            }

            settings.Voices = PopulateVoiceSettingOptions();
        }

        internal string GetAudioFileFromSsml(string ssml, string voice, string style, string rate)
        {

            ssml = AddVoiceToSsml(ssml, voice, style, rate);

            using var sha = SHA256.Create();

            var ssmlHash = BitConverter.ToString(
                sha.ComputeHash(Encoding.UTF8.GetBytes(ssml))
                ).Replace("-", string.Empty);

            var audioFilename = cacheLocation + ssmlHash + ".mp3";

            if (!File.Exists(audioFilename))
            {
                using StringContent request = new(ssml)
                {
                    Headers = {
                        { "obs-plugin", "herald" },
                        { "api-key", ApiKey }
                    }
                };
                
                using var response = httpClient.PostAsync(ApiEndpoint + "/Speak", request).Result;

                if (response.IsSuccessStatusCode)
                {
                    using FileStream fileStream = new FileStream(audioFilename, FileMode.CreateNew);
                    response.Content.ReadAsStream().CopyTo(fileStream);
                    fileStream.Close();
                }
                else
                {
                    throw new PluginException("Herald", "Unable to retrieve audio data.", new Exception(response.StatusCode.ToString() + ": " + response.ReasonPhrase));
                }

            }

            UpdateAndPruneCache(new FileInfo(audioFilename));
                        
            return audioFilename;
        }

        private static string AddVoiceToSsml(string ssml, string voiceName, string styleName, string rate)
        {
            XmlDocument ssmlDoc = new();
            ssmlDoc.LoadXml(ssml);

            var ssmlNamespace = ssmlDoc.DocumentElement.NamespaceURI;
            XmlNamespaceManager ssmlNs = new(ssmlDoc.NameTable);
            ssmlNs.AddNamespace("ssml", ssmlNamespace);
            ssmlNs.AddNamespace("mstts", "http://www.w3.org/2001/mstts");

            var voiceNode = ssmlDoc.SelectSingleNode("/ssml:speak/ssml:voice", ssmlNs);
            voiceNode.Attributes.GetNamedItem("name").Value = voiceName;

            if (!string.IsNullOrWhiteSpace(styleName))
            {
                var expressAsNode = ssmlDoc.CreateElement("express-as", "http://www.w3.org/2001/mstts");
                expressAsNode.SetAttribute("style", styleName);
                expressAsNode.InnerXml = voiceNode.InnerXml;
                voiceNode.InnerXml = expressAsNode.OuterXml;
            }

            if (!string.IsNullOrWhiteSpace(rate))
            {
                var prosodyNode = ssmlDoc.CreateElement("prosody", ssmlNamespace);
                prosodyNode.SetAttribute("rate", rate);
                prosodyNode.InnerXml = voiceNode.InnerXml;
                voiceNode.InnerXml = prosodyNode.OuterXml;
            }

            return ssmlDoc.OuterXml;
        }

        private Dictionary<string, object> PopulateVoiceSettingOptions()
        {
            Dictionary<string, object> voices = new();

            using var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpoint + "/List")
            {
                Headers = {
                    { "obs-plugin", "herald" },
                    { "api-key", ApiKey }
                }
            };            

            var response = httpClient.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                var voiceJson = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);

                var englishSpeakingVoices = from v in voiceJson.RootElement.EnumerateArray()
                                            where v.GetProperty("Locale").GetString().StartsWith("en-")
                                            select v;

                foreach(var voice in englishSpeakingVoices)
                {
                    string demonym = GetDemonymFromLocale(voice.GetProperty("Locale").GetString());

                    voices.Add(
                        demonym + " - " + voice.GetProperty("LocalName").GetString(),
                        voice);

                    if (voice.TryGetProperty("StyleList", out var styles))
                    foreach (var style in styles.EnumerateArray())
                    {
                        voices.Add(
                            demonym + " - " + voice.GetProperty("LocalName").GetString() + " - " + style.GetString(),
                            voice);
                    }
                }
            }
            else
            {
                throw new PluginException("Herald", "Unable to retrieve available voices.", new Exception(response.StatusCode.ToString() + ": " + response.ReasonPhrase));
            }

            return voices;
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

        private void UpdateAndPruneCache(FileInfo currentFile)
        {
            Dictionary<string, CacheData> cacheIndex;

            string cacheIndexFile = cacheLocation + "CacheIndex.json";

            if (File.Exists(cacheIndexFile))
            {
                var indexFileContent = File.ReadAllText(cacheIndexFile);
                try
                {
                    cacheIndex = JsonSerializer.Deserialize<Dictionary<string, CacheData>>(indexFileContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    cacheIndex = new();
                }
            }
            else
            {
                cacheIndex = new();
            }

            if (cacheIndex.ContainsKey(currentFile.Name))
            {
                cacheIndex[currentFile.Name] = new(
                    cacheIndex[currentFile.Name].Created,
                    cacheIndex[currentFile.Name].HitCount + 1
                    );
            }
            else
            {
                cacheIndex.Add(currentFile.Name, new(DateTime.UtcNow, 1));
            }

            var cacheFiles = cacheLocation.GetFiles("*.mp3");
            var currentCacheSize = cacheFiles.Sum(f => f.Length);

            while (currentCacheSize > cacheSize * 1024 * 1024)
            {
                var staleFile = (from file in cacheIndex
                                orderby file.Value.HitCount, file.Value.Created
                                select file.Key).First();

                if (staleFile == currentFile.Name)
                    break;

                File.Delete(cacheLocation + staleFile);
                cacheIndex.Remove(staleFile);
                currentCacheSize -= new FileInfo(cacheLocation + staleFile).Length;
            }

            File.WriteAllText(cacheIndexFile, JsonSerializer.Serialize(cacheIndex));
        }

        public class CacheData
        {
            public CacheData(DateTime Created, int HitCount)
            {
                this.Created = Created;
                this.HitCount = HitCount;
            }
            public DateTime Created { get; set; }
            public int HitCount { get; set; }
        }
    }
}
