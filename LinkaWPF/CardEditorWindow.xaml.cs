using LinkaWPF.Models;
using LinkaWPF.Utils;
using Microsoft.DirectX.AudioVideoPlayback;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для CardEditorWindow.xaml
    /// </summary>
    public partial class CardEditorWindow : Window
    {
        private static readonly DependencyProperty ImageProperty;
        private readonly YandexSpeech _yandexSpeech;
        private readonly string _tempDirPath;
        private bool _withoutSpace;
        private Settings _settings;

        private ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        static CardEditorWindow()
        {
            ImageProperty = DependencyProperty.Register(
                "Image",
                typeof(ImageSource),
                typeof(CardEditorWindow));
        }

        public CardEditorWindow(Settings settings, string tempDirPath, YandexSpeech yandexSpeech, bool withoutSpace) : this(settings, tempDirPath, yandexSpeech, new Card(), withoutSpace)
        {
            acceptButton.Content = "Добавить";
        }

        public CardEditorWindow(Settings settings, string tempDirPath, YandexSpeech yandexSpeech, Card card, bool withoutSpace)
        {
            InitializeComponent();

            Caption = card.Title;
            Image = SetImageFromPath(card.ImagePath);
            
            AudioPath = card.AudioPath;

            _yandexSpeech = yandexSpeech;
            _tempDirPath = tempDirPath;
            _settings = settings;
            voiceSelect.ItemsSource = YandexVoice.VOICES;
            voiceSelect.SelectedItem = YandexVoice.FindById(settings.VoiceId);
            acceptButton.Content = "Изменить";

            _withoutSpace = withoutSpace;

            if (withoutSpace == true)
            {
                panelWithAudioButtons.Visibility = Visibility.Hidden;
                infoAboutAudioTextBlock.Visibility = Visibility.Visible;
            }

            SetCardType(card.CardType);
        }

        private void UploadImage(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Images files(*.jpg,*.png,*.gif)|*.jpg;*.png;*.gif";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
            ImagePath = openFileDialog.FileName;
            Image = SetImageFromPath(openFileDialog.FileName);
        }

        private void CreateImageFromTextClick(object sender, RoutedEventArgs e)
        {
            CreateImageFromText();
        }

        private void CreateImageFromText()
        {
            try
            {
                var imageGenerator = new ImageGenerator();
                var imagePath = string.Format("{0}\\{1}.png", _tempDirPath, Guid.NewGuid());
                imageGenerator.GenerateImage(captionTextBox.Text, imagePath);
                ImagePath = ImagePath;
                Image = SetImageFromPath(imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При создании картинки из текста произошла ошибка (возможно отсутствует интернет соединение)! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveImageClick(object sender, RoutedEventArgs e) {
            if(ImagePath == null)
            {
                MessageBox.Show(this, "Невозможносохранить картинку, которой нет");
                return;
            }
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.FileName = "Картинка." + System.IO.Path.GetExtension( ImagePath);

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return ;

            File.Copy(ImagePath, saveFileDialog.FileName);

        }
        private async Task<bool> UploadAudioFromYandex()
        {
            var result = false;
            if (Caption == null || Caption == string.Empty)
            {
                MessageBox.Show("Поле Title не может быть пустым!", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return result;
            }

            try
            {
                audioPanel.IsEnabled = false;
                acceptButton.IsEnabled = false;
                AudioPath = await _yandexSpeech.GetAudio(captionTextBox.Text, (YandexVoice) voiceSelect.SelectedItem);

                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При загрузке аудио произошла ошибка (возможно отсутствует интернет соединение)! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            audioPanel.IsEnabled = true;
            acceptButton.IsEnabled = true;
            return result;
        }

        private async void uploadFromYandexButton_Click(object sender, RoutedEventArgs e)
        {
            await UploadAudioFromYandex();
        }

        private void UploadAudio(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Audio files(*.ogg,*.mp3,*.wav)|*.ogg;*.mp3;*.wav";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            AudioPath = openFileDialog.FileName;
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            if (AudioPath == null || File.Exists(AudioPath) == false)
            {
                MessageBox.Show("Перед воспроизведением загрузите аудио файл!", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;

            }
            var audio = new Audio(AudioPath);
            audio.Ending += new EventHandler((s, args) => {
                // Разблокируем кнопки после окончания воспроизведения
                audioPanel.IsEnabled = true;
            });

            // Блокируем кнопки перед воспроизведением
            audioPanel.IsEnabled = false;

            audio.Play();
        }

        private void CardTypeChanged(object sender, RoutedEventArgs e)
        {
            if (commonCardTypeRadioButton.IsChecked == true)
            {
                CardType = CardType.Common;

                captionPanel.IsEnabled = true;
                imagePanel.IsEnabled = true;
                audioPanel.IsEnabled = true;
            } else if (spaceCardTypeRadioButton.IsChecked == true)
            {
                CardType = CardType.Space;

                // Блокируем элементы управления
                captionPanel.IsEnabled = false;
                imagePanel.IsEnabled = false;
                audioPanel.IsEnabled = false;

                Caption = "" + '\u2423';

                // Создадим картинку для пробела
                CreateImageFromText();
            } else if (fakeCardTypeRadioButton.IsChecked == true)
            {
                CardType = CardType.Fake;

                // Блокируем элементы управления
                captionPanel.IsEnabled = false;
                imagePanel.IsEnabled = false;
                audioPanel.IsEnabled = false;
            }
        }

        private void SetCardType(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Common:
                    {
                        commonCardTypeRadioButton.IsChecked = true;
                    }
                    break;
                case CardType.Space:
                    {
                        spaceCardTypeRadioButton.IsChecked = true;
                    }
                    break;
                case CardType.Fake:
                    {
                        fakeCardTypeRadioButton.IsChecked = true;
                    }
                    break;
            }
        }

        private async void Accept(object sender, RoutedEventArgs e)
        {
            if (CardType == CardType.Fake)
            {
                Caption = null;
                ImagePath = null;
                AudioPath = null;
            }

            if (CardType != CardType.Fake && (Caption == null || Caption == string.Empty))
            {
                MessageBox.Show("Поле Title не может быть пустым!", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (CardType != CardType.Fake && (ImagePath == null || ImagePath == string.Empty))
            {
                MessageBox.Show("Поле с изображением не может быть пустым!", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (CardType == CardType.Common && _withoutSpace == false && AudioPath == null)
            {
                if (MessageBox.Show("У карточки отсутствует аудио! Создать аудио из текста?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Создаем аудио из тектса
                    await UploadAudioFromYandex();
                }
            }

            DialogResult = true;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private ImageSource SetImageFromPath(string path)
        {
            if (path == null || File.Exists(path) == false) return null;

            ImagePath = path;

            return new BitmapImage(new Uri(path));
        }

        public string Caption
        {
            get { return captionTextBox.Text; }
            private set { captionTextBox.Text = value; }
        }

        public string ImagePath
        {
            get;
            private set;
        }

        public string AudioPath
        {
            get;
            private set;
        }

        public CardType CardType { get; set; }
    }
}
