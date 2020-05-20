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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private IList<Models.Card> _cards;
        private CardButton _selectedCardButton;

        public EditorWindow()
        {
            InitializeComponent();

            _cards = new List<Models.Card>();

            cardBoard.Cards = _cards;
            cardBoard.ClickOnCardButton += ClickOnCardButton;
        }

        private void ClickOnCardButton(object sender, EventArgs e)
        {
            var cardButton = sender as CardButton;

            SelectCard(cardButton);
        }

        private void AddCard(object sender, RoutedEventArgs e)
        {
            var cardEditorWindow = new CardEditorWindow();
            cardEditorWindow.Owner = this;
            cardEditorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (cardEditorWindow.ShowDialog() != true) return;

            var card = new Models.Card(_cards.Count, cardEditorWindow.Caption, cardEditorWindow.ImagePath, cardEditorWindow.AudioPath, cardEditorWindow.WithoutSpace);
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

            if ( rows != cardBoard.Rows) cardBoard.Rows = rows;
            if (columns != cardBoard.Columns) cardBoard.Columns = columns;
        }

        private void SelectCard(CardButton cardButton)
        {
            if (cardButton == null || cardButton.Card == null) return;

            RemoveSelectionCard();

            _selectedCardButton = cardButton;
            _selectedCardButton.Background = Brushes.Yellow;
        }

        private void RemoveSelectionCard()
        {
            if (_selectedCardButton == null) return;

            _selectedCardButton.Background = Brushes.White;
            _selectedCardButton = null;
        }
    }
}
