using Microsoft.DirectX.AudioVideoPlayback;
using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();

            _cards = new List<Models.Card>() {
                // Page one
                new Models.Card(0, "One", "1.png", @"C:\Users\alex\LinkaWPF\LinkaWPF\bin\Debug\audios\one.ogg"),
                new Models.Card(1, "Two", "2.png", @"C:\Users\alex\LinkaWPF\LinkaWPF\bin\Debug\audios\two.ogg"),
                new Models.Card(2, "Three", "3.png", @"C:\Users\alex\LinkaWPF\LinkaWPF\bin\Debug\audios\three.ogg"),
                new Models.Card(3, "Four", "4.png", @"C:\Users\alex\LinkaWPF\LinkaWPF\bin\Debug\audios\four.ogg"),
                new Models.Card(4, "Five", "5.png", @"C:\Users\alex\LinkaWPF\LinkaWPF\bin\Debug\audios\five.ogg"),
                new Models.Card(5, "Six", "6.png", @"C:\Users\alex\LinkaWPF\LinkaWPF\bin\Debug\audios\six.ogg"),
                new Models.Card(6, "Seven", "7.png", @"C:\Users\alex\LinkaWPF\LinkaWPF\bin\Debug\audios\seven.ogg"),
                new Models.Card(7, "Eight", "8.png"),
                new Models.Card(8, "Nine", "9.png"),
                new Models.Card(9, "Nine", "9.png"),
                new Models.Card(10, "Sleep", "sleep.gif"),
                new Models.Card(11, "Sleep", "eat.gif")
            };

            cardBoard.Cards = _cards;
            cardBoard.ClickOnCardButton += cardButton_Click;

            _words = new List<Models.Card>();
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
            // Воспроизведение
            void play(IList<Models.Card> playlist, int startIndex)
            {
                if (playlist == null || startIndex >= playlist.Count) return;
                if (playlist[startIndex].Audio == null)
                {
                    play(playlist, startIndex + 1);
                    return;
                }

                var audio = new Audio(playlist[startIndex].Audio);
                audio.Ending += new EventHandler((obj, evnt) => {
                    play(_words, startIndex);
                });
                audio.Play();
                startIndex++;
            }

            play(_words, 0);
        }
    }
}
