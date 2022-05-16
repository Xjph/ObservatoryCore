using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Observatory.Herald
{
    class SpeechRequestManager
    {
        private HttpClient httpClient;
        private string ApiKey;
        private string ApiEndpoint;
        private DirectoryInfo cacheLocation;
        private int cacheSize;
        private Action<Exception, string> ErrorLogger;
        private ConcurrentDictionary<string, CacheData> cacheIndex;

        internal SpeechRequestManager(
            HeraldSettings settings, HttpClient httpClient, string cacheFolder, Action<Exception, String> errorLogger)
        {
            ApiKey = ObservatoryAPI.ApiKey;
            ApiEndpoint = settings.ApiEndpoint;
            this.httpClient = httpClient;
            cacheSize = Math.Max(settings.CacheSize, 1);
            cacheLocation = new DirectoryInfo(cacheFolder);
            ReadCache();
            ErrorLogger = errorLogger;

            if (!Directory.Exists(cacheLocation.FullName))
            {
                Directory.CreateDirectory(cacheLocation.FullName);
            }

            settings.Voices = PopulateVoiceSettingOptions();
        }

        internal async Task<string> GetAudioFileFromSsml(string ssml, string voice, string style, string rate)
        {

            ssml = AddVoiceToSsml(ssml, voice, style, rate);

            using var sha = SHA256.Create();

            var ssmlHash = BitConverter.ToString(
                sha.ComputeHash(Encoding.UTF8.GetBytes(ssml))
                ).Replace("-", string.Empty);

            var audioFilename = cacheLocation + ssmlHash + ".mp3";

            FileInfo audioFileInfo = null;
            if (File.Exists(audioFilename))
            {
                audioFileInfo = new FileInfo(audioFilename);
                if (audioFileInfo.Length == 0)
                {
                    File.Delete(audioFilename);
                    audioFileInfo = null;
                }
            }


            if (audioFileInfo == null)
            {
                using StringContent request = new(ssml)
                {
                    Headers = {
                        { "obs-plugin", "herald" },
                        { "api-key", ApiKey }
                    }
                };
                
                using var response = await httpClient.PostAsync(ApiEndpoint + "/Speak", request);

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
                audioFileInfo = new FileInfo(audioFilename);
            }

            UpdateAndPruneCache(audioFileInfo);
                        
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
            ssmlNs.AddNamespace("emo", "http://www.w3.org/2009/10/emotionml");

            var voiceNode = ssmlDoc.SelectSingleNode("/ssml:speak/ssml:voice", ssmlNs);
            voiceNode.Attributes.GetNamedItem("name").Value = voiceName;

            if (!string.IsNullOrWhiteSpace(rate))
            {
                var prosodyNode = ssmlDoc.CreateElement("ssml", "prosody", ssmlNamespace);
                prosodyNode.SetAttribute("rate", rate);
                prosodyNode.InnerXml = voiceNode.InnerXml;
                voiceNode.InnerXml = prosodyNode.OuterXml;
            }

            if (!string.IsNullOrWhiteSpace(styleName))
            {
                var expressAsNode = ssmlDoc.CreateElement("mstts", "express-as", "http://www.w3.org/2001/mstts");
                expressAsNode.SetAttribute("style", styleName);
                expressAsNode.InnerXml = voiceNode.InnerXml;
                voiceNode.InnerXml = expressAsNode.OuterXml;
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

        private void ReadCache()
        {
            string cacheIndexFile = cacheLocation + "CacheIndex.json";

            if (File.Exists(cacheIndexFile))
            {
                var indexFileContent = File.ReadAllText(cacheIndexFile);
                try
                {
                    cacheIndex = JsonSerializer.Deserialize<ConcurrentDictionary<string, CacheData>>(indexFileContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    cacheIndex = new();
                    ErrorLogger(ex, "deserializing CacheIndex.json");
                }
            }
            else
            {
                cacheIndex = new();
            }

            // Re-index orphaned files in event of corrupted or lost index.
            var cacheFiles = cacheLocation.GetFiles("*.mp3");
            foreach (var file in cacheFiles.Where(file => !cacheIndex.ContainsKey(file.Name)))
            {
                cacheIndex.TryAdd(file.Name, new(file.CreationTime, 0));
            };
        }

        private void UpdateAndPruneCache(FileInfo currentFile)
        {
            var cacheFiles = cacheLocation.GetFiles("*.mp3");
            if (cacheIndex.ContainsKey(currentFile.Name))
            {
                cacheIndex[currentFile.Name] = new(
                    cacheIndex[currentFile.Name].Created,
                    cacheIndex[currentFile.Name].HitCount + 1
                    );
            }
            else
            {
                cacheIndex.TryAdd(currentFile.Name, new(DateTime.UtcNow, 1));
            }

            var indexedCacheSize = cacheFiles
                .Where(f => cacheIndex.ContainsKey(f.Name))
                .Sum(f => f.Length);

            while (indexedCacheSize > cacheSize * 1024 * 1024)
            {
                var staleFile = (from file in cacheIndex
                                orderby file.Value.HitCount, file.Value.Created
                                select file.Key).First();

                if (staleFile == currentFile.Name)
                    break;

                cacheIndex.TryRemove(staleFile, out _);
            }
        }

        internal async void CommitCache()
        {
            string cacheIndexFile = cacheLocation + "CacheIndex.json";

            System.Diagnostics.Stopwatch stopwatch = new();
            stopwatch.Start();

            // Race condition isn't a concern anymore, but should check this anyway to be safe.
            // (Maybe someone is poking at the file with notepad?)
            while (!IsFileWritable(cacheIndexFile) && stopwatch.ElapsedMilliseconds < 1000)
                await Task.Factory.StartNew(() => System.Threading.Thread.Sleep(100));

            // 1000ms should be more than enough for a conflicting title or detail to complete,
            // if we're still waiting something else is locking the file, just give up.
            if (stopwatch.ElapsedMilliseconds < 1000)
            {
                File.WriteAllText(cacheIndexFile, JsonSerializer.Serialize(cacheIndex));

                // Purge cache from earlier versions, if still present.
                var legacyCache = cacheLocation.GetFiles("*.wav");
                Array.ForEach(legacyCache, file => File.Delete(file.FullName));
            }

            stopwatch.Stop();
        }

        private static bool IsFileWritable(string path)
        {
            try
            {
                using FileStream fs = File.OpenWrite(path);
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private class CacheData
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
