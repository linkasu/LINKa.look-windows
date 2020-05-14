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
        private List<CardButton> _buttons;
        private int _currentPage;
        private int _countPages;
        private int _gridSize;
        private int _rows;
        private int _columns;
        private CircularProgressBar _progress;
        private Storyboard _sb;
        private List<Models.Card> _words;

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

            // Рассчитываем максимальное количество страниц
            this._countPages = Convert.ToInt32(Math.Round(Convert.ToDouble(this._cards.Count) / this._gridSize, 0));

            this._buttons = new List<CardButton>();

            // Создаем кнопки и раскладываем их по клеткам таблицы
            for (var i = 0; i < this._gridSize; i++)
            {
                var button = new CardButton();
                button.Click += new RoutedEventHandler(cardButton_Click);
                button.HazGazeChanged += new RoutedEventHandler(cardButton_HazGazeChanged);
                button.MouseEnter += new MouseEventHandler(cardButton_MouseEnter);
                button.MouseLeave += new MouseEventHandler(cardButton_MouseLeave);

                var row = Convert.ToInt32(Math.Round(Convert.ToDouble(i / this._rows), 0));
                int column = i - (this._rows * row);

                this.gridCard.Children.Add(button);
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                this._buttons.Add(button);
            }

            _progress = new CircularProgressBar();
            _progress.StrokeThickness = 6;
            _progress.HorizontalAlignment = HorizontalAlignment.Center;
            _progress.VerticalAlignment = VerticalAlignment.Center;
            _progress.Visibility = Visibility.Hidden;

            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(3));
            animation.Completed += new EventHandler((o, args) => {                
                _progress.Visibility = Visibility.Hidden;
            });
            Storyboard.SetTarget(animation, _progress);
            Storyboard.SetTargetProperty(animation, new PropertyPath(CircularProgressBar.PercentageProperty));

            _sb = new Storyboard();
            _sb.Children.Add(animation);

            _words = new List<Models.Card>();
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

            if (button.Card == null) return;

            text.Text += " " + button.Card.Title;

            // Добавить карточку в цепочку
            _words.Add(button.Card);
        }

        private void cardButton_HazGazeChanged(object sender, RoutedEventArgs e)
        {
            var button = sender as CardButton;
            startClick(button);
        }

        private void startClick(CardButton button)
        {
            if (_progress.Parent != null)
            {
                (_progress.Parent as Grid).Children.Remove(_progress);
            }

            // Добавляем прогресс на карточку
            button.grid.Children.Add(_progress);

            _progress.Radius = Convert.ToInt32((button.ActualHeight - 20) / 2);
            _progress.Visibility = Visibility.Visible;

            _sb.Stop();
            _sb.Begin();
        }

        private void stopClick()
        {
            _progress.Visibility = Visibility.Hidden;
        }

        private void cardButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            var button = sender as CardButton;

            startClick(button);
        }

        private void cardButton_MouseLeave(object sender, RoutedEventArgs e)
        {
            stopClick();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Очистка текстового поля
            text.Text = "";
            text.Focus();

            // Очистить цепочку слов
            _words.Clear();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Удалить последнее слово из текстового поля
            var end = text.Text.LastIndexOf(' ');
            text.Text = end <= 0 ? "" : text.Text.Substring(0, end);
            text.Select(text.Text.Length, 0);
            text.Focus();

            // Удалить последнее слово из цепочки слов
            _words.RemoveAt(_words.Count - 1);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Воспроизведение
            void play(IList<Models.Card> playlist, int startIndex)
            {
                if (playlist == null || (playlist != null && playlist[startIndex].Audio == null)) return;
                var audio = new Audio(playlist[startIndex].Audio);
                audio.Ending += new EventHandler((obj, evnt) => {
                    if (++startIndex >= playlist.Count) return;

                    play(_words, startIndex);
                });
                audio.Play();
            }

            play(_words, 0);
        }
    }
}
