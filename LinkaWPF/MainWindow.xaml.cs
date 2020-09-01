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
using System.Windows.Controls.Primitives;
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
        private Player _player;

        public MainWindow(Settings settings)
        {
            StaticServer.instance.ReportEvent("startupApp") ;
            
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

            _words = new List<Card>();
            try
            {
                var joystick = new Joysticks();
                joystick.JoystickButtonDown += Joystick_JoystickButtonDown;
            } catch(Exception e)
            {

            }
            _player = new Player(_yandexSpeech);
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
            _settings.IsPlayAudioFromCard = settings.IsPlayAudioFromCard;
            _settings.IsPageButtonVisible = settings.IsPageButtonVisible;

            _settings.IsJoystickEnabled = settings.IsJoystickEnabled;
            _settings.IsKeyboardEnabled = settings.IsKeyboardEnabled;
            _settings.IsMouseEnabled = settings.IsMouseEnabled;

            _settings.SettingsLoader.SaveToFile(_settings.ConfigFilePath, _settings);
        }

        private void NextElement(FocusNavigationDirection direction)
        {
            var request = new TraversalRequest(direction);

            var elementWithFocus = Keyboard.FocusedElement as UIElement;

            if (elementWithFocus != null) elementWithFocus.MoveFocus(request);
        }

        private void RunAction(string keyName)
        {
            string action;
            if (Settings.Keys.TryGetValue(keyName, out action) == false) return;
            StaticServer.instance.ReportEvent("MoveCursor", new Dictionary<string, string> { { "action", action } });

            switch (action)
            {
                case "MoveSelectorRight":
                    {
                        NextElement(FocusNavigationDirection.Next);
                    }
                    break;
                case "MoveSelectorLeft":
                    {
                        NextElement(FocusNavigationDirection.Previous);
                    }
                    break;
                case "MoveSelectorUp":
                    {
                        NextElement(FocusNavigationDirection.Up);
                    }
                    break;
                case "MoveSelectorDown":
                    {
                        NextElement(FocusNavigationDirection.Down);
                    }
                    break;
                case "Enter":
                    {
                        /*if (cardBoard.SelectedCardButton == null) return;

                        pressCardButton(cardBoard.SelectedCardButton);*/

                        var button = Keyboard.FocusedElement as Button;

                        if (button != null) button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                    break;
            }
        }
        private void Joystick_JoystickButtonDown(object sender, string buttonName)
        {
            if (Settings.IsJoystickEnabled == true)
            {
                RunAction(buttonName);
                StaticServer.instance.ReportEvent("JoystickAction", new Dictionary<string, string>()
                {
                    {"buttonName", buttonName }
                });

            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Settings.IsKeyboardEnabled == true)
            {
                RunAction(e.Key.ToString());
            }
            e.Handled = true;
            base.OnKeyDown(e);
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
            
            StaticServer.instance.ReportEvent("CardSelected") ;
            if (_settings.IsPlayAudioFromCard == true)
            {
                var cards = new List<Card>();
                cards.Add(cardButton.Card);
                _player.Play(cards);
            }
            else
            {
                Stream str = Properties.Resources.type;
                System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                snd.Play(); 
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

                // Добавить карточку в цепочку
                _words.Add(cardButton.Card);
            }
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

            // Очистить цепочку слов
            _words.Clear();
        }

        private void removeLastWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (WithoutSpace == true)
            {
                if (text.Text.Length > 0) text.Text = text.Text.Remove(text.Text.Length - 1, 1);
            }
            else
            {
                // Удалить последнее слово из текстового поля
                var end = text.Text.LastIndexOf(' ');
                text.Text = end <= 0 ? "" : text.Text.Substring(0, end);

                // Переставить курсор в конец строки
                text.Select(text.Text.Length, 0);

                // Удалить последнее слово из цепочки слов
                if (_words.Count > 0) _words.RemoveAt(_words.Count - 1);
            }
        }

        private void pronounceWordsButton_Click(object sender, RoutedEventArgs e)
        {
            StaticServer.instance.ReportEvent("Pronounce");

            if (WithoutSpace == true)
            {
                _player.Play(text.Text);
            }
            else
            {
                _player.Play(_words);
            }
        }

        public void LoadCardSet(string path)
        {
            overflowGrid.Visibility = Visibility.Hidden;

            StaticServer.instance.ReportEvent("LoadCardSet", new Dictionary<string, string>
            {
                {"path", path }
            });
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
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\LINKa";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            LoadCardSet(openFileDialog.FileName);
        }

        private void OpenInEditor_Click(object sender, RoutedEventArgs e)
        {
            StaticServer.instance.ReportEvent("OpenInEditor");

            ChangeMode(CurrentFileName);
        }

        private void OpenEditor_Click(object sender, RoutedEventArgs e)
        {
            StaticServer.instance.ReportEvent("OpenEditor");

            ChangeMode(null);
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            StaticServer.instance.ReportEvent("OpenSettings");

            var settingsWindow = new SettingsWindow(Settings);
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settingsWindow.Owner = this;
            if (settingsWindow.ShowDialog() == false) return;

            Settings = settingsWindow.Settings;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            StaticServer.instance.ReportEvent("Exit");

            Close();
        }

        private void changeGazeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.IsHasGazeEnabled = !Settings.IsHasGazeEnabled;
        }
    }
}
