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
        private HeraldSettings heraldSettings;
        private HttpClient httpClient;

        public SpeechRequestManager(HeraldSettings heraldSettings, HttpClient httpClient)
        {
            this.heraldSettings = heraldSettings;
            this.httpClient = httpClient;
        }

        internal string GetAudioFileFromSsml(string ssml, string voice, string style)
        {
            throw new NotImplementedException();
        }
    }
}
