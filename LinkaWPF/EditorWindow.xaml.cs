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

            _cards = new List<Models.Card>()
            {
                new Models.Card(0, "Test", null)
            };

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
            _cards.Add(new Models.Card(_cards.Count, "Test", null));
            cardBoard.Cards = null;
            cardBoard.Cards = _cards;
        }

        private void EditCard(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveCard(object sender, RoutedEventArgs e)
        {
            if (_selectedCardButton == null || _selectedCardButton.Card == null) return;

            _cards.Remove(_selectedCardButton.Card);
            RemoveSelectionCard();
            cardBoard.Cards = null;
            cardBoard.Cards = _cards;
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
