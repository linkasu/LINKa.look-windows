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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private IList<Card> _cards;
        private CardButton _selectedCardButton;

        public EditorWindow()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _cards = new List<Card>();

            cardBoard.Cards = _cards;
            cardBoard.ClickOnCardButton += ClickOnCardButton;
            cardBoard.CountPagesChanged += CardBoard_CountPagesChanged;
            cardBoard.CurrentPageChanged += CardBoard_CurrentPageChanged;
        }

        private void CardBoard_CurrentPageChanged(object sender, EventArgs e)
        {
            UpdatePageInfo();
            RemoveSelectionCard();
        }

        private void CardBoard_CountPagesChanged(object sender, EventArgs e)
        {
            UpdatePageInfo();
        }

        private void ClickOnCardButton(object sender, EventArgs e)
        {
            var cardButton = sender as CardButton;

            SelectCard(cardButton);
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            if (_selectedCardButton == null || _selectedCardButton.Card == null || _selectedCardButton.Card.AudioPath == null || File.Exists(_selectedCardButton.Card.AudioPath) == false) return;

            var audio = new Audio(_selectedCardButton.Card.AudioPath);
            playButton.IsEnabled = false;
            audio.Ending += (s, args) => { playButton.IsEnabled = true; };
            audio.Play();
        }

        private void AddCard(object sender, RoutedEventArgs e)
        {
            var cardEditorWindow = new CardEditorWindow();
            cardEditorWindow.Owner = this;
            cardEditorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (cardEditorWindow.ShowDialog() != true) return;

            var card = new Card(_cards.Count, cardEditorWindow.Caption, cardEditorWindow.ImagePath, cardEditorWindow.AudioPath, cardEditorWindow.WithoutSpace);
            _cards.Add(card);

            cardBoard.Update(_cards);
        }

        private void EditCard(object sender, RoutedEventArgs e)
        {
            if (_selectedCardButton == null || _selectedCardButton.Card == null) return;

            var index = _cards.IndexOf(_selectedCardButton.Card);

            var cardEditorWindow = new CardEditorWindow(_selectedCardButton.Card.Title, _selectedCardButton.Card.WithoutSpace, _selectedCardButton.Card.ImagePath, _selectedCardButton.Card.AudioPath);
            cardEditorWindow.Owner = this;
            cardEditorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (cardEditorWindow.ShowDialog() != true) return;

            _cards[index].Title = cardEditorWindow.Caption;
            _cards[index].WithoutSpace = cardEditorWindow.WithoutSpace;
            _cards[index].ImagePath = cardEditorWindow.ImagePath;
            _cards[index].AudioPath = cardEditorWindow.AudioPath;

            cardBoard.UpdateCard(index, _cards[index]);
        }

        private void RemoveCard(object sender, RoutedEventArgs e)
        {
            if (_selectedCardButton == null || _selectedCardButton.Card == null) return;

            _cards.Remove(_selectedCardButton.Card);
            RemoveSelectionCard();

            cardBoard.Update(_cards);
        }

        private void ChangeGridSize(object sender, RoutedEventArgs e)
        {
            var rows = Convert.ToInt32(rowsText.Text);
            var columns = Convert.ToInt32(columnsText.Text);

            if (rows != cardBoard.Rows) cardBoard.Rows = rows;
            if (columns != cardBoard.Columns) cardBoard.Columns = columns;
        }

        private void SelectedCardChanged(Card card)
        {
            playButton.IsEnabled = card == null || card.AudioPath == null || card.AudioPath == string.Empty ? false : true;
            editButton.IsEnabled = card == null ? false : true;
            deleteButton.IsEnabled = card == null ? false : true;
            moveToLeftButton.IsEnabled = card == null ? false : true;
            moveToRightButton.IsEnabled = card == null ? false : true;
        }

        private void SelectCard(CardButton cardButton)
        {
            if (cardButton == null || cardButton.Card == null) return;

            RemoveSelectionCard();

            _selectedCardButton = cardButton;
            _selectedCardButton.Background = Brushes.Yellow;

            SelectedCardChanged(_selectedCardButton.Card);
        }

        private void RemoveSelectionCard()
        {
            if (_selectedCardButton == null) return;

            _selectedCardButton.Background = Brushes.White;
            _selectedCardButton = null;

            SelectedCardChanged(null);
        }

        private void PrevPage(object sender, RoutedEventArgs e)
        {
            cardBoard.PrevPage();
        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            cardBoard.NextPage();
        }

        private void MoveToLeft(object sender, RoutedEventArgs e)
        {
            // Переместить карточку влево
            if (_selectedCardButton == null || _selectedCardButton.Card == null || _cards.Count == 0) return;

            var index = _cards.IndexOf(_selectedCardButton.Card);

            if (index <= 0 || index >= _cards.Count) return;

            var prevCard = _cards[index - 1];
            _cards[index - 1] = _cards[index];
            _cards[index] = prevCard;

            cardBoard.Update(_cards);
        }

        private void MoveToRight(object sender, RoutedEventArgs e)
        {
            // Переместить карточку вправо
            if (_selectedCardButton == null || _selectedCardButton.Card == null || _cards.Count == 0) return;

            var index = _cards.IndexOf(_selectedCardButton.Card);

            if (index < 0 || index >= _cards.Count - 1) return;

            var nextCard = _cards[index + 1];
            _cards[index + 1] = _cards[index];
            _cards[index] = nextCard;

            cardBoard.Update(_cards);
        }

        private void SaveCardSet(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "Linka files(*.linka)|*.linka";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            try
            {
                var cardSetFile = new CardSetFile(cardBoard.Columns, cardBoard.Rows, _cards);
                var cardSetLoader = new CardSetLoader();
                cardSetLoader.SaveToFile(saveFileDialog.FileName, cardSetFile);

                MessageBox.Show(this, "Набор успешно сохранен!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При сохранении набора произошла ошибка! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCardSet(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Linka files(*.linka)|*.linka";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            try
            {
                var destPath = Environment.CurrentDirectory + "\\temp\\" + Guid.NewGuid();

                var cardSetLoader = new CardSetLoader();
                var cardSetFile = cardSetLoader.LoadFromFile(openFileDialog.FileName, destPath);

                cardBoard.Columns = cardSetFile.Columns;
                cardBoard.Rows = cardSetFile.Rows;

                _cards = cardSetFile.Cards;
                foreach (var card in _cards)
                {
                    if (card.ImagePath != null && card.ImagePath != string.Empty) card.ImagePath = destPath + "\\" + card.ImagePath;
                    if (card.AudioPath != null && card.AudioPath != string.Empty) card.AudioPath = destPath + "\\" + card.AudioPath;
                }
                cardBoard.Update(_cards);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При загрузке набора произошла ошибка! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }           
        }

        private void UpdatePageInfo()
        {
            pageInfoTextBlock.Text = string.Format("Текущая страница: {0} из {1}", cardBoard.CurrentPage + 1, cardBoard.CountPages);
        }
    }
}
