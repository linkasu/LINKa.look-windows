using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.Interaction;

namespace LinkaWPF
{
    public class Settings
    {
        public Dictionary<string, string> Keys { get; set; }

        public string TempDirPath { get; set; }

        public string ConfigFilePath { get; set; }

        public SettingsLoader SettingsLoader { get; set; }

        public YandexSpeech YandexSpeech { get; set; }

        public Host Host { get; set; }
    }
}
