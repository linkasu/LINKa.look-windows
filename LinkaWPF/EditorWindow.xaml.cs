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
        private static readonly DependencyProperty WithoutSpaceProperty;
        private IList<Card> _cards;
        private CardButton _selectedCardButton;

        private bool WithoutSpace
        {
            get { return (bool)GetValue(WithoutSpaceProperty); }
            set { SetValue(WithoutSpaceProperty, value); }
        }

        static EditorWindow()
        {
            WithoutSpaceProperty = DependencyProperty.Register(
                "WithoutSpace",
                typeof(bool),
                typeof(EditorWindow),
                new PropertyMetadata(false, new PropertyChangedCallback(OnWithoutSpaceChanged)));
        }

        private static void OnWithoutSpaceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var window = sender as EditorWindow;
            window.WithoutSpace = (bool)args.NewValue;
            window.ChangeStatusPlayButton();
            window.IsEdited = true;
        }

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

            if (cardBoard.CountPages > 1)
            {
                prevPageButton.IsEnabled = true;
                nextPageButton.IsEnabled = true;
            }
            else
            {
                prevPageButton.IsEnabled = false;
                nextPageButton.IsEnabled = false;
            }
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
            CreateCard();
        }

        private void EditCard(object sender, RoutedEventArgs e)
        {
            if (_selectedCardButton == null || _selectedCardButton.Card == null) return;

            var index = _cards.IndexOf(_selectedCardButton.Card);

            var card = new Card() {
                Title = _selectedCardButton.Card.Title,
                ImagePath = _selectedCardButton.Card.ImagePath,
                AudioPath = _selectedCardButton.Card.AudioPath,
                CardType = _selectedCardButton.Card.CardType
            };

            var cardEditorWindow = new CardEditorWindow(card, WithoutSpace);
            cardEditorWindow.Owner = this;
            cardEditorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (cardEditorWindow.ShowDialog() != true) return;

            _cards[index].Title = cardEditorWindow.Caption;
            _cards[index].ImagePath = cardEditorWindow.ImagePath;
            _cards[index].AudioPath = cardEditorWindow.AudioPath;

            UpdateCardBoard(_cards, index);
        }

        private void RemoveCard(object sender, RoutedEventArgs e)
        {
            if (_selectedCardButton == null || _selectedCardButton.Card == null) return;

            _cards.Remove(_selectedCardButton.Card);
            RemoveSelectionCard();

            UpdateCardBoard(_cards);
        }

        private void ChangeGridSize(object sender, RoutedEventArgs e)
        {
            var rows = Convert.ToInt32(rowsText.Text);
            var columns = Convert.ToInt32(columnsText.Text);

            if (rows != cardBoard.Rows)
            {
                cardBoard.Rows = rows;
                IsEdited = true;
            }

            if (columns != cardBoard.Columns)
            {
                cardBoard.Columns = columns;
                IsEdited = true;
            }
        }

        private void SelectedCardChanged(Card card)
        {
            ChangeStatusPlayButton();
            editButton.IsEnabled = card == null ? false : true;
            deleteButton.IsEnabled = card == null ? false : true;
            moveToLeftButton.IsEnabled = card == null ? false : true;
            moveToRightButton.IsEnabled = card == null ? false : true;
        }

        private void SelectCard(CardButton cardButton)
        {
            if (cardButton == null || cardButton.Card == null)
            {
                CreateCard();
                return;
            }

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

            UpdateCardBoard(_cards);
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

            UpdateCardBoard(_cards);
        }

        private void SaveCardSet_Click(object sender, RoutedEventArgs e)
        {
            SaveCardSet();
        }

        private bool SaveCardSet()
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "Linka files(*.linka)|*.linka";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

            try
            {
                var cardSetFile = new CardSetFile(cardBoard.Columns, cardBoard.Rows, WithoutSpace, _cards);
                var cardSetLoader = new CardSetLoader();
                cardSetLoader.SaveToFile(saveFileDialog.FileName, cardSetFile);

                IsEdited = false;

                MessageBox.Show(this, "Набор успешно сохранен!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При сохранении набора произошла ошибка! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        private void LoadCardSet_Click(object sender, RoutedEventArgs e)
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

                WithoutSpace = cardSetFile.WithoutSpace;

                _cards = cardSetFile.Cards;
                foreach (var card in _cards)
                {
                    if (card.ImagePath != null && card.ImagePath != string.Empty) card.ImagePath = destPath + "\\" + card.ImagePath;
                    if (card.AudioPath != null && card.AudioPath != string.Empty) card.AudioPath = destPath + "\\" + card.AudioPath;
                }
                UpdateCardBoard(_cards);

                IsEdited = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При загрузке набора произошла ошибка! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }           
        }

        private void CreateCard()
        {
            var cardEditorWindow = new CardEditorWindow(WithoutSpace);
            cardEditorWindow.Owner = this;
            cardEditorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (cardEditorWindow.ShowDialog() != true) return;

            var card = new Card(_cards.Count, cardEditorWindow.Caption, cardEditorWindow.ImagePath, cardEditorWindow.AudioPath, cardEditorWindow.CardType);
            _cards.Add(card);

            UpdateCardBoard(_cards);
        }

        private void UpdatePageInfo()
        {
            pageInfoTextBlock.Text = string.Format("Текущая страница: {0} из {1}", cardBoard.CurrentPage + 1, cardBoard.CountPages);
        }

        private void UpdateCardBoard(IList<Card> cards)
        {
            cardBoard.Update(cards);
            IsEdited = true;
        }

        private void UpdateCardBoard(IList<Card> cards, int index)
        {
            cardBoard.UpdateCard(index, cards[index]);
            IsEdited = true;
        }

        private void ChangeStatusPlayButton()
        {
            if (_selectedCardButton == null || _selectedCardButton.Card == null) return;

            var card = _selectedCardButton.Card;

            if (WithoutSpace == true)
            {
                playButton.IsEnabled = false;
            }
            else
            {
                playButton.IsEnabled = card == null || card.AudioPath == null || card.AudioPath == string.Empty ? false : true;
            }
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsEdited == true)
            {
                var result = MessageBox.Show(this, "Сохранить изменения?", "Сохранение", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch(result)
                {
                    case MessageBoxResult.Yes:
                        {
                            // Сохраняем изменения и выходим
                            e.Cancel = !SaveCardSet();
                        }
                        break;
                    case MessageBoxResult.No:
                        {
                            e.Cancel = false;
                        }break;
                    default:
                        {
                            e.Cancel = true;
                        }break;
                }
            } 
        }

        protected bool IsEdited { get; set; }
    }
}
