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

            // TODO: Заменить на загрузку из конфига
             _yandexSpeech = new YandexSpeech("4e68a4e5-b590-448d-9a66-f3d8f2854348");

            _cards = new List<Models.Card>() {
                // Page one
                new Models.Card(0, "Один", "1.png", @"audios\one.ogg"),
                new Models.Card(1, "Два", "2.png", @"audios\two.ogg"),
                new Models.Card(2, "Три", "3.png", @"audios\three.ogg"),
                new Models.Card(3, "Четыре", "4.png", @"audios\four.ogg"),
                new Models.Card(4, "Пять", "5.png", @"audios\five.ogg"),
                new Models.Card(5, "Шесть", "6.png", @"audios\six.ogg"),
                new Models.Card(6, "Семь", "7.png", @"audios\seven.ogg"),
                new Models.Card(7, "Восемь", "8.png"),
                new Models.Card(8, "Девять", "9.png"),
                new Models.Card(9, "Девять", "9.png"),
                new Models.Card(10, "Спать", "sleep.gif"),
                new Models.Card(11, "Есть", "eat.gif")
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
