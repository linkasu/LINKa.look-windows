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

            Init();
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
            cardBoard.Update(cards);
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

        public int CountPages
        {
            get { return _countPages; }
            private set
            {
                _countPages = value;
                CountPagesChanged?.Invoke(this, new EventArgs());
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                _currentPage = value;
                CurrentPageChanged?.Invoke(this, new EventArgs());
            }
        }
        #endregion

        #region Events
        // Events
        public event EventHandler ClickOnCardButton;

        public event EventHandler CountPagesChanged;

        public event EventHandler CurrentPageChanged;

        #endregion

        #region Methods
        // Methods
        private void InitGrid()
        {
            CurrentPage = 0;
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
            if (Cards == null || Cards.Count <= 0)
            {
                CountPages = 1;
                CurrentPage = 0;
                return;
            }

            // Рассчитываем максимальное количество страниц
            CountPages = Convert.ToInt32(Math.Ceiling((double)Cards.Count / _gridSize));

            if (CurrentPage >= CountPages) CurrentPage = CountPages - 1;
        }

        private void Render()
        {
            if (Cards == null)
            {
                for(var i = 0; i < _buttons.Count; i++)
                {
                    _buttons[i].Card = null;
                }
                return;
            }

            for (var i = CurrentPage * _gridSize; i < CurrentPage * _gridSize + _gridSize; i++)
            {
                Models.Card card = null;
                if (i >= 0 && i < Cards.Count)
                {
                    card = Cards[i];
                }
                var count = i - CurrentPage * _gridSize;
                _buttons[count].Card = card;
            }
        }

        public void Update(IList<Models.Card> cards)
        {
            if (Cards != cards)
            {
                Cards = cards;
            }

            InitPages();
            Render();
        }

        public void UpdateCard(int index, Models.Card card)
        {
            if (Cards == null || index < 0 || index >= Cards.Count) return;

            Cards[index] = card;

            // Выясним на какой странице находится карточка
            var page = Convert.ToInt32(Math.Round((double)index / _gridSize));

            // Находится ли карточка на текущей странице
            if (page == CurrentPage)
            {
                // Вычисляем индекс кнопки на которой находится карточка
                var indexOfButtons = index - CurrentPage * _gridSize;

                // Обновляем карточку на кнопке
                _buttons[indexOfButtons].Card = null;
                _buttons[indexOfButtons].Card = card;
            }
        }

        public bool NextPage()
        {
            if (CountPages <= 1) return false;

            CurrentPage++;

            if (CurrentPage >= CountPages) CurrentPage = 0;

            Render();

            GC.Collect();

            return true;
        }

        public bool PrevPage()
        {
            if (CountPages <= 1) return false;

            CurrentPage--;

            if (CurrentPage < 0) CurrentPage = CountPages - 1;

            Render();

            GC.Collect();

            return true;
        }

        private void Init()
        {
            InitGrid();
            InitCardButtons();
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
