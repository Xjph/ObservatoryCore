using System;
using System.Net.Http;

namespace Observatory
{
    public sealed class HttpClient
    {
        private HttpClient()
        { }

        private static readonly Lazy<System.Net.Http.HttpClient> lazy = new Lazy<System.Net.Http.HttpClient>(() => new System.Net.Http.HttpClient());

        public static System.Net.Http.HttpClient Client
        {
            get
            {
                return lazy.Value;
            }
        }

        public static string GetString(string url)
        {
            return lazy.Value.GetStringAsync(url).Result;
        }

        public static HttpResponseMessage SendRequest(HttpRequestMessage request)
        {
            return lazy.Value.SendAsync(request).Result;
        }

        public static System.Threading.Tasks.Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            return lazy.Value.SendAsync(request);
        }
    }
}