using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Tobii.Interaction;
using Tobii.Interaction.Wpf;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Models.CardModel> _cards;
        private List<Button> _buttons;
        private int _currentPage;
        private int _countPages;
        private int _gridSize;
        private int _rows;
        private int _columns;

        private Host _host;
        private WpfInteractorAgent _wpfInteractorAgent;
        public MainWindow()
        {
            InitializeComponent();

            this.Init();
            this.Render();
        }

        private void Init()
        {
            this._currentPage = 0;
            this._rows = 6;
            this._columns = 6;
            this._gridSize = this._rows * this._columns;

            for(var i = 0; i < this._rows; i++)
            {
                var rowDefinition = new RowDefinition();
                gridCard.RowDefinitions.Add(rowDefinition);
            }

            for (var i = 0; i < this._columns; i++)
            {
                var columnDefinition = new ColumnDefinition();
                gridCard.ColumnDefinitions.Add(columnDefinition);
            }

            this._cards = new List<Models.CardModel>() {
                // Page one
                new Models.CardModel("One", "1.png"),
                new Models.CardModel("Two", "2.png"),
                new Models.CardModel("Three", "3.png"),
                new Models.CardModel("Four", "4.png"),
                new Models.CardModel("Five", "5.png"),
                new Models.CardModel("Six", "6.png"),
                new Models.CardModel("Seven", "7.png"),
                new Models.CardModel("Eight", "8.png"),
                new Models.CardModel("Nine", "9.png"),
                new Models.CardModel("Sleep", "sleep.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Sleep", "eat.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                new Models.CardModel("Nine", "b64dee73545128824e6c31ddb946e03e.gif"),
                
                // Page two
                new Models.CardModel("Ten", "10.png"),
                new Models.CardModel("Eleven", "11.png"),
                new Models.CardModel("Twelve", "12.png"),
                new Models.CardModel("Thirteen", "13.png"),
                new Models.CardModel("Fourteen", "14.png"),
                new Models.CardModel("Fiveteen", "15.png"),
                new Models.CardModel("Sixteen", "16.png"),
                new Models.CardModel("Seventeen", "17.png"),
                new Models.CardModel("Eighteen", "18.png"),

                // Page three
                new Models.CardModel("Nineteen", "19.png"),
                new Models.CardModel("Twenty", "20.png"),
                new Models.CardModel("Twenty-one", "21.png"),
                new Models.CardModel("Twenty-two", "22.png"),
                new Models.CardModel("Twenty-three", "23.png"),
                new Models.CardModel("Twenty-four", "24.png"),
                new Models.CardModel("Twenty-five", "25.png"),
                new Models.CardModel("Twenty-six", "26.png"),
                new Models.CardModel("Twenty-seven", "27.png")
            };

            // Рассчитываем максимальное количество страниц
            this._countPages = Convert.ToInt32(Math.Round(Convert.ToDouble(this._cards.Count) / this._gridSize, 0));

            this._buttons = new List<Button>();

            // Создаем кнопки и раскладываем их по клеткам таблицы
            for (var i = 0; i < this._gridSize; i++)
            {
                var style = this.FindResource("PaintButton") as Style;
                var button = new Button();
                button.Click += new RoutedEventHandler(this.cardButton_Click);
                button.Style = style;

                var row = Convert.ToInt32(Math.Round(Convert.ToDouble(i / this._rows), 0));
                int column = i - (this._rows * row);

                this.gridCard.Children.Add(button);
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                this._buttons.Add(button);
            }
        }

        private void NextPage()
        {
            if (this._currentPage == this._countPages - 1)
            {
                this._currentPage = 0;
            } else
            {
                this._currentPage++;
            }
            this.Render();
        }

        private void PrevPage()
        {
            if (this._currentPage - 1 < 0)
            {
                this._currentPage = this._countPages - 1;
            } else
            {
                this._currentPage--;
            }
            this.Render();
        }

        private void Render()
        {
            for (var i = this._currentPage * this._gridSize; i < this._currentPage * this._gridSize + this._gridSize; i++)
            {
                Models.CardModel card = null;
                if (i >= 0 && i < this._cards.Count)
                {                    
                    card = this._cards[i];
                }
                var count = i - this._currentPage * this._gridSize;
                this._buttons[count].DataContext = card;
            }
        }

        private void cardButton_Click(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            text.Text = ((Models.CardModel)button.DataContext).Title;
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

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var mediaElement = (MediaElement)sender;
            mediaElement.Position = new TimeSpan(0, 0, 1);
            mediaElement.Play();
        }

        /*
         * TODO:
         * 1. Сделать кнопки для перелистывания 1/2 от размера карточки
         * 2. Добавить обработчик нажатия на карточку, в котором прописать добавление текста с карточки в текст бокс
         * */
    }
}
