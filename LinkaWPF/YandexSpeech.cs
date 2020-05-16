using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LinkaWPF
{
    public class YandexSpeech
    {
        private readonly string _key;
        public YandexSpeech(string key)
        {
            _key = key;
        }

        public async Task<string> GetAudio(string text)
        {
            var client = new HttpClient();

            var values = new Dictionary<string, string>
            {
                { "text", text },
                { "key", _key }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://tts.voicetech.yandex.net/tts", content);
            if (response.IsSuccessStatusCode == true)
            {
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                var audioFile = string.Format(@"temp\{0}.ogg", System.Guid.NewGuid());
                File.WriteAllBytes(audioFile, responseBytes);

                return audioFile;
            }

            return null;
        }
    }
}
