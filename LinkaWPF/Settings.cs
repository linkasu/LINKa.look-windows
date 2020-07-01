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
    public class Settings : INotifyPropertyChanged, ICloneable
    {
        private bool _isHasGazeEnabled;
        private bool _isAnimatedClickEnabled;
        private double _clickDelay;
        private bool _isPlayAudioFromCard;
        private bool _isPageButtonVisible;
        private bool _isKeyboardEnabled;
        private bool _isJoystickEnabled;
        private bool _isMouseEnabled;

        public Dictionary<string, string> Keys { get; set; }

        public string TempDirPath { get; set; }

        public string ConfigFilePath { get; set; }

        public SettingsLoader SettingsLoader { get; set; }

        public YandexSpeech YandexSpeech { get; set; }

        public Host Host { get; set; }

        public bool IsHasGazeEnabled
        {
            get { return _isHasGazeEnabled; }
            set
            {
                _isHasGazeEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsAnimatedClickEnabled
        {
            get { return _isAnimatedClickEnabled; }
            set
            {
                _isAnimatedClickEnabled = value;
                OnPropertyChanged();
            }
        }

        public double ClickDelay
        {
            get { return _clickDelay; }
            set {
                _clickDelay = value;
                OnPropertyChanged();
            }
        }

        public bool IsPlayAudioFromCard
        {
            get { return _isPlayAudioFromCard; }
            set
            {
                _isPlayAudioFromCard = value;
                OnPropertyChanged();
            }
        }

        public bool IsPageButtonVisible
        {
            get { return _isPageButtonVisible; }
            set
            {
                _isPageButtonVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsKeyboardEnabled
        {
            get { return _isKeyboardEnabled; }
            set
            {
                _isKeyboardEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsJoystickEnabled
        {
            get { return _isJoystickEnabled; }
            set
            {
                _isJoystickEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsMouseEnabled
        {
            get { return _isMouseEnabled; }
            set
            {
                _isMouseEnabled = value;
                OnPropertyChanged();
            }
        }
            

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
