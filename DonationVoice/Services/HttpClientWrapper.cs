using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DonationVoice.Services
{
    public class HttpClientWrapper
    {
        private readonly HttpClient _client;

        public HttpClientWrapper()
        {
            _client = new HttpClient();
        }

        public async Task<Stream> GetStream(string uri) => await _client.GetStreamAsync(uri);

        public async Task<string> GetVoiceUrl(string text, string voice)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "text", text },
                { "voice", voice }
            };
            using var urlParameters = new FormUrlEncodedContent(parameters);
            var result = await _client.PostAsync("https://streamlabs.com/polly/speak", urlParameters);

            return JsonConvert.DeserializeObject<Response>(await result.Content.ReadAsStringAsync()).SpeakUrl;
        }
    }

    public struct Response
    {
        [JsonProperty("speak_url")]
        public string SpeakUrl { get; set; }
    }
}
