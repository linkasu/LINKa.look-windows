using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tobii.Interaction;

namespace LinkaWPF
{
    public class Settings : INotifyPropertyChanged
    {
        private double _delayClick;

        public Dictionary<string, string> Keys { get; set; }

        public string TempDirPath { get; set; }

        public string ConfigFilePath { get; set; }

        public SettingsLoader SettingsLoader { get; set; }

        public YandexSpeech YandexSpeech { get; set; }

        public Host Host { get; set; }

        public bool IsHazGazeEnabled { get; set; }

        public bool IsAnimatedClickEnabled { get; set; }

        public double ClickDelay
        {
            get { return _delayClick; }
            set {
                _delayClick = value;
                OnPropertyChanged("ClickDelay");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
