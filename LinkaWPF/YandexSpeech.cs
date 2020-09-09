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
        private readonly string _tempPath;

        public YandexSpeech(string key, string tempPath)
        {
            _key = key;
            _tempPath = tempPath;
        }

        public async Task<string> GetAudio(string text)
        {
            var client = new HttpClient();
//            client.Timeout = TimeSpan.FromMilliseconds(10000);
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

                var audioFile = string.Format(@"{0}\{1}.ogg", _tempPath, System.Guid.NewGuid());
                File.WriteAllBytes(audioFile, responseBytes);

                return audioFile;
            }

            return null;
        }
    }
}
