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
        private YandexSpeech _yandexSpeech;
        private readonly string _tempPath;

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

        public CardEditorWindow() : this("", false, null, null)
        {
            acceptButton.Content = "Добавить";
        }

        public CardEditorWindow(string caption, bool withoutSpace, string imagePath, string audioPath)
        {
            InitializeComponent();

            // TODO: Перенести в родительский класс и передавать параметром
            // Создать директорию для временных файлов
            _tempPath = Environment.CurrentDirectory + "\\temp\\";
            Directory.CreateDirectory(_tempPath);

            // TODO: Заменить на загрузку из конфига
            _yandexSpeech = new YandexSpeech("4e68a4e5-b590-448d-9a66-f3d8f2854348", _tempPath);

            captionTextBox.Text = caption;
            withoutSpaceCheckBox.IsChecked = withoutSpace;
            Image = SetImageFromPath(imagePath);
            AudioPath = audioPath;

            acceptButton.Content = "Изменить";
        }

        private void UploadImage(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Images files(*.jpg,*.png,*.gif)|*.jpg;*.png;*.gif";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            ImagePath = openFileDialog.FileName;

            Image = SetImageFromPath(ImagePath);
        }

        private void CreateImageFromText(object sender, RoutedEventArgs e)
        {
            try
            {
                var imageGenerator = new ImageGenerator();
                ImagePath = string.Format("{0}\\{1}.png", _tempPath, Guid.NewGuid());
                imageGenerator.GenerateImage(captionTextBox.Text, ImagePath);

                Image = SetImageFromPath(ImagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При создании картинки из текста произошла ошибка (возможно отсутствует интернет соединение)! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UploadAudioFromYandex(object sender, RoutedEventArgs e)
        {
            if (captionTextBox.Text == null || captionTextBox.Text == string.Empty)
            {
                MessageBox.Show("Поле Title не может быть пустым!", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            try
            {
                uploadFromYandexButton.IsEnabled = false;
                uploadFromFileButton.IsEnabled = false;
                playButton.IsEnabled = false;
                acceptButton.IsEnabled = false;
                AudioPath = await _yandexSpeech.GetAudio(captionTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При загрузке аудио произошла ошибка (возможно отсутствует интернет соединение)! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            uploadFromYandexButton.IsEnabled = true;
            uploadFromFileButton.IsEnabled = true;
            playButton.IsEnabled = true;
            acceptButton.IsEnabled = true;
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
                uploadFromYandexButton.IsEnabled = true;
                uploadFromFileButton.IsEnabled = true;
                playButton.IsEnabled = true;
            });

            // Блокируем кнопки перед воспроизведением
            uploadFromYandexButton.IsEnabled = false;
            uploadFromFileButton.IsEnabled = false;
            playButton.IsEnabled = false;

            audio.Play();
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            if (captionTextBox.Text == null || captionTextBox.Text == string.Empty)
            {
                MessageBox.Show("Поле Title не может быть пустым!", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
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

            return new BitmapImage(new Uri(path));
        }

        public string Caption
        {
            get { return captionTextBox.Text; }
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

        public bool WithoutSpace
        {
            get { return withoutSpaceCheckBox.IsChecked == null || withoutSpaceCheckBox.IsChecked == false ? false : true; }
        }
    }
}
