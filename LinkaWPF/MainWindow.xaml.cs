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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Models.Card> _cards;
        private List<Models.Card> _words;
        private YandexSpeech _yandexSpeech;

        public MainWindow()
        {
            InitializeComponent();

            // Создать директорию для временных файлов
            var tempPath = Environment.CurrentDirectory + "\\temp\\";
            Directory.CreateDirectory(tempPath);

            // TODO: Заменить на загрузку из конфига
            _yandexSpeech = new YandexSpeech("4e68a4e5-b590-448d-9a66-f3d8f2854348", tempPath);

            _cards = new List<Models.Card>() {
                // Page one
                new Models.Card(0, "Один", string.Format("{0}\\images\\1.png", Environment.CurrentDirectory), string.Format("{0}\\audios\\one.ogg", Environment.CurrentDirectory)),
                new Models.Card(1, "Два", string.Format("{0}\\images\\2.png", Environment.CurrentDirectory), string.Format("{0}\\audios\\two.ogg", Environment.CurrentDirectory)),
                new Models.Card(2, "Три", string.Format("{0}\\images\\3.png", Environment.CurrentDirectory), string.Format("{0}\\audios\\three.ogg", Environment.CurrentDirectory)),
                new Models.Card(3, "Четыре", string.Format("{0}\\images\\4.png", Environment.CurrentDirectory), string.Format("{0}\\audios\\four.ogg", Environment.CurrentDirectory)),
                new Models.Card(4, "Пять", string.Format("{0}\\images\\5.png", Environment.CurrentDirectory), string.Format("{0}\\audios\\five.ogg", Environment.CurrentDirectory)),
                new Models.Card(5, "Шесть", string.Format("{0}\\images\\6.png", Environment.CurrentDirectory), string.Format("{0}\\audios\\six.ogg", Environment.CurrentDirectory)),
                new Models.Card(6, "Семь", string.Format("{0}\\images\\7.png", Environment.CurrentDirectory), string.Format("{0}\\audios\\seven.ogg", Environment.CurrentDirectory)),
                new Models.Card(7, "Восемь", string.Format("{0}\\images\\8.png", Environment.CurrentDirectory)),
                new Models.Card(8, "Девять", string.Format("{0}\\images\\9.png", Environment.CurrentDirectory)),
                new Models.Card(9, "Девять", string.Format("{0}\\images\\9.png", Environment.CurrentDirectory)),
                new Models.Card(10, "Спать", string.Format("{0}\\images\\sleep.gif", Environment.CurrentDirectory)),
                new Models.Card(11, "Есть", string.Format("{0}\\images\\eat.gif", Environment.CurrentDirectory))
            };

            cardBoard.Cards = _cards;
            cardBoard.ClickOnCardButton += cardButton_Click;

            _words = new List<Models.Card>();
        }

        private void ClearCach()
        {
            var allfiles = Directory.GetFiles("temp");
            foreach (var filename in allfiles)
            {
                File.Delete(filename);
            }
        }

        private void cardButton_Click(object sender, EventArgs e)
        {
            var cardButton = sender as CardButton;

            if (cardButton.Card == null) return;

            text.Text += (text.Text != string.Empty ? " " : string.Empty) + cardButton.Card.Title;

            // Переставить курсор в конец строки
            text.Select(text.Text.Length, 0);

            // Передать фокус текстовому полю
            text.Focus();

            // Добавить карточку в цепочку
            _words.Add(cardButton.Card);
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
            _words.RemoveAt(_words.Count - 1);
        }

        private void pronounceWordsButton_Click(object sender, RoutedEventArgs e)
        {
            var player = new Player(_yandexSpeech);

            player.Play(_words);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Удаляем закешированные аудиофайлы
            ClearCach();
        }
    }
}
