using LinkaWPF.Models;
using LinkaWPF.Properties;
using Microsoft.DirectX.AudioVideoPlayback;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Tobii.Interaction;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IList<Card> _cards;
        private IList<Card> _words;
        private readonly string _tempDirPath;
        private readonly YandexSpeech _yandexSpeech;
        private readonly Host _host;
        private Settings _settings;

        public MainWindow(Settings settings)
        {
            InitializeComponent();

            _tempDirPath = settings.TempDirPath;

            _yandexSpeech = settings.YandexSpeech;

            _host = settings.Host;

            ChangeSettings(settings);

            _cards = new List<Card>();

            cardBoard.ClickOnCardButton += cardButton_Click;
            cardBoard.CountPagesChanged += CardBoard_CountPagesChanged;
            cardBoard.CurrentPageChanged += CardBoard_CurrentPageChanged;
            cardBoard.Cards = _cards;
            cardBoard.Host = _host;

            _words = new List<Card>();

            KeyUp += MainWindow_KeyUp;

            var joystick = new Joysticks();
            joystick.JoystickButtonDown += Joystick_JoystickButtonDown;
        }

        public Settings Settings
        {
            get { return _settings; }
            set { ChangeSettings(value); }
        }

        public void ChangeSettings(Settings settings)
        {
            if (_settings == null)
            {
                _settings = settings;
                DataContext = _settings;
            }

            _settings.IsHasGazeEnabled = settings.IsHasGazeEnabled;
            _settings.IsAnimatedClickEnabled = settings.IsAnimatedClickEnabled;
            _settings.ClickDelay =  settings.ClickDelay;

            //cardBoard.IsAnimatedClickEnabled = _settings.IsAnimatedClickEnabled;
            //cardBoard.IsHazGazeEnabled = settings.IsHasGazeEnabled;

            _settings.SettingsLoader.SaveToFile(_settings.ConfigFilePath, _settings);
        }

        private void RunAction(string keyName)
        {
            string action;
            if (Settings.Keys.TryGetValue(keyName, out action) == false) return;

            switch (action)
            {
                case "MoveSelectorRight":
                    {
                        cardBoard.MoveSelectorRight();
                    }
                    break;
                case "MoveSelectorLeft":
                    {
                        cardBoard.MoveSelectorLeft();
                    }
                    break;
                case "MoveSelectorUp":
                    {
                        cardBoard.MoveSelectorUp();
                    }
                    break;
                case "MoveSelectorDown":
                    {
                        cardBoard.MoveSelectorDown();
                    }
                    break;
                case "Enter":
                    {
                        if (cardBoard.SelectedCardButton == null) return;

                        pressCardButton(cardBoard.SelectedCardButton);
                    }
                    break;
            }
        }
        private void Joystick_JoystickButtonDown(object sender, string buttonName)
        {
            RunAction(buttonName);
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            RunAction(e.Key.ToString());
        }

        private void CardBoard_CurrentPageChanged(object sender, EventArgs e)
        {
            UpdatePageInfo();
        }

        private void CardBoard_CountPagesChanged(object sender, EventArgs e)
        {
            UpdatePageInfo();

            if (cardBoard.CountPages > 1)
            {
                prevButton.IsEnabled = true;
                nextButton.IsEnabled = true;
            }
            else
            {
                prevButton.IsEnabled = false;
                nextButton.IsEnabled = false;
            }
        }

        private void UpdatePageInfo()
        {
            pageInfoTextBlock.Text = string.Format("Текущая страница: {0} из {1}", cardBoard.CurrentPage + 1, cardBoard.CountPages);
        }

        private void pressCardButton(CardButton cardButton)
        {
            if (cardButton.Card == null) return;

            if (WithoutSpace == false)
                text.Text += (text.Text != string.Empty ? " " : string.Empty);

            if (cardButton.Card.CardType == CardType.Space)
            {
                text.Text += " ";
            }
            else
            {
                text.Text += cardButton.Card.Title;
            }

            // Переставить курсор в конец строки
            text.Select(text.Text.Length, 0);

            // Передать фокус текстовому полю
            text.Focus();

            // Добавить карточку в цепочку
            _words.Add(cardButton.Card);
        }

        private void cardButton_Click(object sender, EventArgs e)
        {
            var cardButton = sender as CardButton;
            pressCardButton(cardButton);
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            // Prev
            cardBoard.PrevPage();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            // Next
            cardBoard.NextPage();
        }

        private void clearTextBoxButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка текстового поля
            text.Text = "";
            text.Focus();

            // Очистить цепочку слов
            _words.Clear();
        }

        private void removeLastWordButton_Click(object sender, RoutedEventArgs e)
        {
            // Удалить последнее слово из текстового поля
            var end = text.Text.LastIndexOf(' ');
            text.Text = end <= 0 ? "" : text.Text.Substring(0, end);

            // Переставить курсор в конец строки
            text.Select(text.Text.Length, 0);

            // Передать фокус текстовому полю
            text.Focus();

            // Удалить последнее слово из цепочки слов
            if (_words.Count > 0) _words.RemoveAt(_words.Count - 1);
        }

        private void pronounceWordsButton_Click(object sender, RoutedEventArgs e)
        {
            var player = new Player(_yandexSpeech);

            if (WithoutSpace == true)
            {
                player.Play(text.Text);
            }
            else
            {
                player.Play(_words);
            }
        }

        public void LoadCardSet(string path)
        {
            try
            {
                var destPath = _tempDirPath + Guid.NewGuid() + "\\";

                var cardSetLoader = new CardSetLoader();
                var cardSetFile = cardSetLoader.LoadFromFile(path, destPath);

                cardBoard.Columns = cardSetFile.Columns;
                cardBoard.Rows = cardSetFile.Rows;

                WithoutSpace = cardSetFile.WithoutSpace;

                _cards = cardSetFile.Cards;
                foreach (var card in _cards)
                {
                    if (card.ImagePath != null && card.ImagePath != string.Empty) card.ImagePath = destPath + card.ImagePath;
                    if (card.AudioPath != null && card.AudioPath != string.Empty) card.AudioPath = destPath + card.AudioPath;
                }
                cardBoard.Update(_cards);

                CurrentFileName = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При загрузке набора произошла ошибка! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public bool WithoutSpace { get; set; }

        public Func<string, bool> ChangeMode;

        public string CurrentFileName { get; set; }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Linka files(*.linka)|*.linka";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            LoadCardSet(openFileDialog.FileName);
        }

        private void OpenInEditor_Click(object sender, RoutedEventArgs e)
        {
            ChangeMode(CurrentFileName);
        }

        private void OpenEditor_Click(object sender, RoutedEventArgs e)
        {
            ChangeMode(null);
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(Settings);
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settingsWindow.Owner = this;
            if (settingsWindow.ShowDialog() == false) return;

            Settings = settingsWindow.Settings;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void changeGazeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.IsHasGazeEnabled = !Settings.IsHasGazeEnabled;
        }
    }
}
