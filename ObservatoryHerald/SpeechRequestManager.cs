using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Observatory.Herald
{
    class SpeechRequestManager
    {
        private HttpClient httpClient;
        private string ApiKey;

        public SpeechRequestManager(HttpClient httpClient)
        {
            ApiKey = ObservatoryAPI.ApiKey;
            this.httpClient = httpClient;
        }

        internal string GetAudioFileFromSsml(string ssml, string voice, string style)
        {
            throw new NotImplementedException();
        }
    }
}
