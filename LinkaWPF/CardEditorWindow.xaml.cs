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

        public CardEditorWindow()
        {
            InitializeComponent();

            // Создать директорию для временных файлов
            var tempPath = Environment.CurrentDirectory + "\\temp\\";
            Directory.CreateDirectory(tempPath);

            // TODO: Заменить на загрузку из конфига
            _yandexSpeech = new YandexSpeech("4e68a4e5-b590-448d-9a66-f3d8f2854348", tempPath);
        }

        private void UploadImage(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Images files(*.jpg,*.png,*.gif)|*.jpg;*.png;*.gif";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            ImagePath = openFileDialog.FileName;

            Image = new BitmapImage(new Uri(ImagePath));
        }

        private async void UploadAudioFromYandex(object sender, RoutedEventArgs e)
        {
            if (title.Text == null || title.Text == string.Empty)
            {
                MessageBox.Show("Поле Title не может быть пустым!", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            uploadFromYandexButton.IsEnabled = false;
            uploadFromFileButton.IsEnabled = false;
            AudioPath = await _yandexSpeech.GetAudio(title.Text);

            uploadFromYandexButton.IsEnabled = true;
            uploadFromFileButton.IsEnabled = true;
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
            }
            var audio = new Audio(AudioPath);
            audio.Ending += new EventHandler((s, args) => { playButton.IsEnabled = true; });
            playButton.IsEnabled = false;
            audio.Play();
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            if (title.Text == null || title.Text == string.Empty)
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

        public string Title
        {
            get { return title.Text; }
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
            get { return withoutSpaceCheckbox.IsChecked == null || withoutSpaceCheckbox.IsChecked == false ? false : true; }
        }
    }
}
