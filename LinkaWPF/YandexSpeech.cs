using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Tobii.Interaction.Wpf;

namespace LinkaWPF
{
    public class YandexSpeech
    {
        private readonly string _tempPath;
        private readonly Settings _settings;

        public YandexSpeech(string tempPath, Settings settings)
        {
            _tempPath = tempPath;
            _settings = settings;
        }
        
        public async Task<string> GetAudio(string text)
        {
            
            return await GetAudio(text, YandexVoice.FindById( _settings.VoiceId));
        }
        public async Task<string> GetAudio(string text, YandexVoice voice )
        {
            var client = new HttpClient();
//            client.Timeout = TimeSpan.FromMilliseconds(10000);
            var values = new Dictionary<string, string>
            {
                { "text", text },
                {"voice", voice.Id }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("http://linka.su:5443/voice", content);
            if (response.IsSuccessStatusCode == true)
            {
                var responseBytes = await response.Content.ReadAsByteArrayAsync();

                var audioFile = string.Format(@"{0}\{1}.mp3", _tempPath, System.Guid.NewGuid());
                File.WriteAllBytes(audioFile, responseBytes);

                return audioFile;
            }

            return null;
        }
    }
}
