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
        private List<CardButton> _buttons;
        private int _currentPage;
        private int _countPages;
        private int _gridSize;
        private int _rows;
        private int _columns;

        public MainWindow()
        {
            InitializeComponent();

            Init();
            Render();
        }

        private void Init()
        {
            this._currentPage = 0;
            this._rows = 6;
            this._columns = 6;
            this._gridSize = this._rows * this._columns;

            for (var i = 0; i < this._rows; i++)
            {
                var rowDefinition = new RowDefinition();
                gridCard.RowDefinitions.Add(rowDefinition);
            }

            for (var i = 0; i < this._columns; i++)
            {
                var columnDefinition = new ColumnDefinition();
                gridCard.ColumnDefinitions.Add(columnDefinition);
            }

            this._cards = new List<Models.Card>() {
                // Page one
                new Models.Card(0, "One", "1.png"),
                new Models.Card(1, "Two", "2.png"),
                new Models.Card(2, "Three", "3.png"),
                new Models.Card(3, "Four", "4.png"),
                new Models.Card(4, "Five", "5.png"),
                new Models.Card(5, "Six", "6.png"),
                new Models.Card(6, "Seven", "7.png"),
                new Models.Card(7, "Eight", "8.png"),
                new Models.Card(8, "Nine", "9.png"),
                new Models.Card(9, "Nine", "9.png"),
                new Models.Card(10, "Sleep", "sleep.gif"),
                new Models.Card(11, "Sleep", "eat.gif")
            };

            // Рассчитываем максимальное количество страниц
            this._countPages = Convert.ToInt32(Math.Round(Convert.ToDouble(this._cards.Count) / this._gridSize, 0));

            this._buttons = new List<CardButton>();

            // Создаем кнопки и раскладываем их по клеткам таблицы
            for (var i = 0; i < this._gridSize; i++)
            {
                var button = new CardButton();
                button.Click += new RoutedEventHandler(this.cardButton_Click);

                var row = Convert.ToInt32(Math.Round(Convert.ToDouble(i / this._rows), 0));
                int column = i - (this._rows * row);

                this.gridCard.Children.Add(button);
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                this._buttons.Add(button);
            }
        }

        private void Render()
        {
            for (var i = this._currentPage * this._gridSize; i < this._currentPage * this._gridSize + this._gridSize; i++)
            {
                Models.Card card = null;
                if (i >= 0 && i < this._cards.Count)
                {
                    card = this._cards[i];
                }
                var count = i - this._currentPage * this._gridSize;
                this._buttons[count].Card = card;
            }
        }

        private void NextPage()
        {
            if (this._currentPage == this._countPages - 1)
            {
                this._currentPage = 0;
            }
            else
            {
                this._currentPage++;
            }
            Render();
        }

        private void PrevPage()
        {
            if (this._currentPage - 1 < 0)
            {
                this._currentPage = this._countPages - 1;
            }
            else
            {
                this._currentPage--;
            }
            Render();
        }

        private void cardButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as CardButton;
            text.Text = button.Card.Title;
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            // Prev
            this.PrevPage();

            GC.Collect();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            // Next
            this.NextPage();

            GC.Collect();
        }
    }
}
