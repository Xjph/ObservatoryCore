using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Observatory.Herald
{
    class SpeechRequestManager
    {
        private HttpClient httpClient;
        private string ApiKey;
        private string ApiEndpoint;
        private DirectoryInfo cacheLocation;
        

        internal SpeechRequestManager(HeraldSettings settings, HttpClient httpClient)
        {
            ApiKey = ObservatoryAPI.ApiKey;
            ApiEndpoint = settings.ApiEndpoint;
            this.httpClient = httpClient;
            cacheLocation = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + $"{Path.DirectorySeparatorChar}ObservatoryCore{Path.DirectorySeparatorChar}ObservatoryHerald{Path.DirectorySeparatorChar}");
            
            if (!Directory.Exists(cacheLocation.FullName))
            {
                Directory.CreateDirectory(cacheLocation.FullName);
            }



            settings.Voices = PopulateVoiceSettingOptions();
        }

        internal string GetAudioFileFromSsml(string ssml, string voice, string style)
        {
            using StringContent request = new(ssml) 
            { 
                Headers = { 
                    { "obs-plugin", "herald" }, 
                    { "api-key", ApiKey }
                }
            };


            var response = httpClient.PostAsync(ApiEndpoint, request).Result;
            throw new NotImplementedException();
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
    }
}
