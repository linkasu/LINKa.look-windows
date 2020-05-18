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

        public CardBoard()
        {
            InitializeComponent();

            Init(); ;
        }

        #region Properties
        // Properties
        public static readonly DependencyProperty CardsProperty =
            DependencyProperty.Register("Cards", typeof(IList<Models.Card>), typeof(CardBoard), new PropertyMetadata(null, new PropertyChangedCallback(OnCardsChanged)));

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(CardBoard), new PropertyMetadata(3, new PropertyChangedCallback(GridSizeChanged)));

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(CardBoard), new PropertyMetadata(3, new PropertyChangedCallback(GridSizeChanged)));

        private static void OnCardsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CardBoard cardBoard = sender as CardBoard;
            var cards = (IList<Models.Card>)args.NewValue;

            cardBoard.Cards = cards;

            if (cards == null) return;

            cardBoard.InitPages();
            cardBoard.Render();
        }

        private static void GridSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CardBoard cardBoard = sender as CardBoard;
            cardBoard.Init();
        }

        public IList<Models.Card> Cards
        {
            get { return (IList<Models.Card>)GetValue(CardsProperty); }
            set { SetValue(CardsProperty, value); }
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set
            {
                if (value <= 0) value = 1;
                SetValue(ColumnsProperty, value);
            }
        }

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set
            {
                if (value <= 0) value = 1;
                SetValue(RowsProperty, value);
            }
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
            _gridSize = Rows * Columns;

            if (grid.Children.Count != 0) grid.Children.Clear();
            if (grid.RowDefinitions.Count != 0) grid.RowDefinitions.Clear();
            if (grid.ColumnDefinitions.Count != 0) grid.ColumnDefinitions.Clear();

            for (var i = 0; i < Rows; i++)
            {
                var rowDefinition = new RowDefinition();
                grid.RowDefinitions.Add(rowDefinition);
            }

            for (var i = 0; i < Columns; i++)
            {
                var columnDefinition = new ColumnDefinition();
                grid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        private void InitCardButtons()
        {
            if (_buttons == null)
            {
                _buttons = new List<CardButton>();
            } else
            {
                _buttons.Clear();
            }

            // Создаем кнопки и раскладываем их по клеткам таблицы
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    var button = CreateCardButton();
                    button.Click += new RoutedEventHandler(CardButton_Click);
                    button.HazGazeChanged += new RoutedEventHandler(CardButton_HazGazeChanged);
                    button.MouseEnter += CardButton_MouseEnter;
                    button.MouseLeave += CardButton_MouseLeave;

                    grid.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);

                    _buttons.Add(button);
                }
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

        private void Init()
        {
            InitGrid();
            InitCardButtons();

            if (Cards == null) return;

            InitPages();
            Render();
        }

        protected virtual CardButton CreateCardButton()
        {
            return new CardButton();
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            PressOnCardButton(sender);
        }

        protected void PressOnCardButton(object sender)
        {
            ClickOnCardButton?.Invoke(sender, new EventArgs());
        }

        protected virtual void CardButton_HazGazeChanged(object sender, RoutedEventArgs e)
        {
        }

        protected virtual void CardButton_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        protected virtual void CardButton_MouseLeave(object sender, MouseEventArgs e)
        {
        }
        #endregion
    }
}
