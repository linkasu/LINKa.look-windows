using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkaWPF
{
    public class SettingsLoader
    {
        public void SaveToFile(string configFile, Settings settings)
        {
            var settingsFile = new SettingsFile() {
                Keys = settings.Keys,
                IsHasGazeEnabled = settings.IsHasGazeEnabled,
                IsAnimatedClickEnabled = settings.IsAnimatedClickEnabled,
                ClickDelay = settings.ClickDelay,
                IsPlayAudioFromCard = settings.IsPlayAudioFromCard
            };

            string json = JsonConvert.SerializeObject(settingsFile, Formatting.Indented);

            using (var writer = new StreamWriter(configFile))
            {
                writer.WriteLine(json);
            }
        }

        public Settings LoadFromFile(string configFile)
        {
            using (var file = File.OpenText(configFile))
            {
                var serializer = new JsonSerializer();
                var settingsFile = (SettingsFile)serializer.Deserialize(file, typeof(SettingsFile));
                return new Settings() {
                    ConfigFilePath = configFile,
                    Keys = settingsFile.Keys,
                    IsHasGazeEnabled = settingsFile.IsHasGazeEnabled ?? true,
                    IsAnimatedClickEnabled = settingsFile.IsAnimatedClickEnabled ?? true,
                    ClickDelay = settingsFile.ClickDelay ?? 3,
                    IsPlayAudioFromCard = settingsFile.IsPlayAudioFromCard ?? false
                };
            }
        }
    }
}
