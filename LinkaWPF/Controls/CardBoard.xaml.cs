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

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для CardBoard.xaml
    /// </summary>
    public partial class CardBoard : UserControl
    {
        private IList<CardButton> _buttons;
        private int _currentPage;
        private int _countPages;
        private int _gridSize;
        private int _rows;
        private int _columns;
        private AnimatedDelayedClick _delayedClick;

        public CardBoard()
        {
            InitializeComponent();

            _delayedClick = new AnimatedDelayedClick(3);
            _delayedClick.Ended += _delayedClick_Ended;

            InitGrid();
        }

        #region Properties
        // Properties
        public static readonly DependencyProperty CardsProperty =
            DependencyProperty.Register("Cards", typeof(IList<Models.Card>), typeof(CardBoard), new PropertyMetadata(null, new PropertyChangedCallback(OnCardsChanged)));

        private static void OnCardsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CardBoard cardBoard = sender as CardBoard;
            cardBoard.Cards = (IList<Models.Card>)args.NewValue;

            cardBoard.InitCardButtons();
            cardBoard.InitPages();

            cardBoard.Render();
        }

        public IList<Models.Card> Cards
        {
            get { return (IList<Models.Card>)GetValue(CardsProperty); }
            set { SetValue(CardsProperty, value); }
        }
#endregion

#region Events
        // Events
        public event EventHandler ClickOnCardButton;

#endregion

#region Methods
        // Methods
        private void InitGrid()
        {
            _currentPage = 0;
            _rows = 6;
            _columns = 6;
            _gridSize = _rows * _columns;

            for (var i = 0; i < _rows; i++)
            {
                var rowDefinition = new RowDefinition();
                grid.RowDefinitions.Add(rowDefinition);
            }

            for (var i = 0; i < _columns; i++)
            {
                var columnDefinition = new ColumnDefinition();
                grid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        private void InitCardButtons()
        {
            grid.Children.Clear();

            _buttons = new List<CardButton>();

            // Создаем кнопки и раскладываем их по клеткам таблицы
            for (var i = 0; i < _gridSize; i++)
            {
                var button = new CardButton();
                button.Click += new RoutedEventHandler(cardButton_Click);
                button.HazGazeChanged += new RoutedEventHandler(cardButton_HazGazeChanged);
                button.MouseEnter += Button_MouseEnter;
                button.MouseLeave += Button_MouseLeave;

                var row = Convert.ToInt32(Math.Round(Convert.ToDouble(i / _rows), 0));
                int column = i - (_rows * row);

                grid.Children.Add(button);
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                _buttons.Add(button);
            }
        }

        private void InitPages()
        {
            // Рассчитываем максимальное количество страниц
            _countPages = Convert.ToInt32(Math.Round(Convert.ToDouble(Cards.Count) / _gridSize, 0));
        }

        private void Render()
        {
            for (var i = _currentPage * _gridSize; i < _currentPage * _gridSize + _gridSize; i++)
            {
                Models.Card card = null;
                if (i >= 0 && i < Cards.Count)
                {
                    card = Cards[i];
                }
                var count = i - _currentPage * _gridSize;
                _buttons[count].Card = card;
            }
        }

        public void NextPage()
        {
            if (_countPages == 0) return;

            if (_currentPage == _countPages - 1)
            {
                _currentPage = 0;
            }
            else
            {
                _currentPage++;
            }
            Render();

            GC.Collect();
        }

        public void PrevPage()
        {
            if (_countPages == 0) return;

            if (_currentPage - 1 < 0)
            {
                _currentPage = _countPages - 1;
            }
            else
            {
                _currentPage--;
            }
            Render();

            GC.Collect();
        }

        private void cardButton_Click(object sender, RoutedEventArgs e)
        {
            clickOnCardButton(sender);
        }

        private void clickOnCardButton(object sender)
        {
            EventArgs e = new EventArgs();
            ClickOnCardButton?.Invoke(sender, e);
        }

        private void cardButton_HazGazeChanged(object sender, RoutedEventArgs e)
        {
            var button = sender as CardButton;
            if (button.Card == null) return;

            _delayedClick.Start(button);
        }

        private void StopClick()
        {
            _delayedClick.Stop();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            var button = sender as CardButton;
            if (button.Card == null) return;

            _delayedClick.Start(button);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            _delayedClick.Stop();
        }

        private void _delayedClick_Ended(object sender, EventArgs e)
        {
            ClickOnCardButton?.Invoke(sender, e);
        }
        #endregion
    }
}
