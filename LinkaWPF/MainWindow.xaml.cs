using LinkaWPF.Models;
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
        private IList<Card> _cards;
        private IList<Card> _words;
        private readonly string _tempDirPath;
        private readonly YandexSpeech _yandexSpeech;

        public MainWindow(string tempDirPath, YandexSpeech yandexSpeech)
        {
            InitializeComponent();

            _tempDirPath = tempDirPath;

            _yandexSpeech = yandexSpeech;

            _cards = new List<Card>();

            cardBoard.ClickOnCardButton += cardButton_Click;
            cardBoard.CountPagesChanged += CardBoard_CountPagesChanged;
            cardBoard.CurrentPageChanged += CardBoard_CurrentPageChanged;
            cardBoard.Cards = _cards;

            _words = new List<Card>();

            KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                // Следующая карточка
                cardBoard.SelectNextCard();
            } else if (e.Key == Key.Left)
            {
                // Предыдущая карточка
                cardBoard.SelectPrevCard();
            }
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
            if (_words.Count > 0) _words.RemoveAt(_words.Count - 1);
        }

        private void pronounceWordsButton_Click(object sender, RoutedEventArgs e)
        {
            var player = new Player(_yandexSpeech);

            player.Play(_words);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При загрузке набора произошла ошибка! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public bool WithoutSpace { get; set; }
    }
}
