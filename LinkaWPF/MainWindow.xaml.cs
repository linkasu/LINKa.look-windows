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
        private List<Button> _buttons;
        private int _currentPage;
        private int _countPages;
        private int _gridSize;
        private int _rows;
        private int _columns;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 2);

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
                new Models.Card("One", "1.png"),
                new Models.Card("Two", "2.png"),
                new Models.Card("Three", "3.png"),
                new Models.Card("Four", "4.png"),
                new Models.Card("Five", "5.png"),
                new Models.Card("Six", "6.png"),
                new Models.Card("Seven", "7.png"),
                new Models.Card("Eight", "8.png"),
                new Models.Card("Nine", "9.png"),
                new Models.Card("Nine", "9.png"),
                new Models.Card("Sleep", "sleep.gif"),
                new Models.Card("Sleep", "eat.gif")
            };

            // Рассчитываем максимальное количество страниц
            this._countPages = Convert.ToInt32(Math.Round(Convert.ToDouble(this._cards.Count) / this._gridSize, 0));

            this._buttons = new List<Button>();

            // Создаем кнопки и раскладываем их по клеткам таблицы
            for (var i = 0; i < this._gridSize; i++)
            {
                var style = this.FindResource("CardButton") as Style;
                var button = new Button();
                button.Click += new RoutedEventHandler(this.cardButton_Click);
                button.MouseMove += new MouseEventHandler(this.cardButton_moveMove);
                button.MouseEnter += new MouseEventHandler(this.cardButton_mouseEnter);
                button.MouseLeave += new MouseEventHandler(this.cardButton_mouseLeave);
                button.Style = style;

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
                this._buttons[count].DataContext = card;
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

        private void cardButton_HasGazeChanged(object sender, Tobii.Interaction.Wpf.HasGazeChangedRoutedEventArgs e)
        {
            text.Text = "HasGazeChanged";

            if (e.Source != null)
            {
                //выдает ошибку что не может привести бордер к баттон
             //   var button = ((Button)e.Source);
               // text.Text = ((Models.Card)button.DataContext).Title;
            } else
            {
                text.Text = "HasGazeChanged";
            }
        }

        private void cardButton_moveMove(object sender, MouseEventArgs e)
        {
            // var button = ((Button)sender);
            // text.Text = ((Models.Card)button.DataContext).Title;
        }

        private void cardButton_mouseEnter(object sender, MouseEventArgs e)
        {
            // var button = ((Button)sender);
            // text.Text = ((Models.Card)button.DataContext).Title;

            //_timer.Start();
            //_timer.Tick += Timer_Tick;
            //_timer.Tag = sender;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //_timer.Stop();
            //if(_timer.Tag != null)
            //{
            //    var button = ((Button)_timer.Tag);
            //    text.Text = ((Models.Card)button.DataContext).Title;
            //}
        }

        private void cardButton_mouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void cardButton_Click(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            text.Text = ((Models.Card)button.DataContext).Title;
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
