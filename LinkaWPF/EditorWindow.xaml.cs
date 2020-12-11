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
        private readonly string _tempDirPath;
        private YandexSpeech _yandexSpeech;
        private String Description;

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

        public EditorWindow(Settings settings, string tempDirPath, YandexSpeech yandexSpeech)
        {
            _settings = settings;
            InitializeComponent();

            // Создать директорию для временных файлов
            _tempDirPath = tempDirPath;

            _yandexSpeech = yandexSpeech;

            _cards = new List<Card>();

            cardBoard.Cards = _cards;
            cardBoard.ClickOnCardButton += ClickOnCardButton;
            cardBoard.CountPagesChanged += CardBoard_CountPagesChanged;
            cardBoard.CurrentPageChanged += CardBoard_CurrentPageChanged;
            cardBoard.SelectedCardChanged += CardBoard_SelectedCardChanged;
            cardBoard.Edited += CardBoard_Edited;
        }

        private void CardBoard_Edited(object sender, EventArgs e)
        {
            IsEdited = true;
        }

        private void CardBoard_SelectedCardChanged(object sender, EventArgs e)
        {
            ChangeStatusPlayButton();
            var isNull = cardBoard.SelectedCard  == null ? false : true;
            editButton.IsEnabled = isNull;
            deleteButton.IsEnabled = isNull;
            moveToLeftButton.IsEnabled = isNull;
            moveToRightButton.IsEnabled = isNull;
            moveToUpButton.IsEnabled = isNull;
            moveToDownButton.IsEnabled = isNull;
        }

        private void CardBoard_CurrentPageChanged(object sender, EventArgs e)
        {
            UpdatePageInfo();
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

            if (cardButton == null || cardButton.Card == null)
            {
                CreateCard();
                return;
            }

            var index = _cards.IndexOf(cardButton.Card);

            cardBoard.SelectCard(index);
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            if (SelectedCardButton == null || SelectedCardButton.Card == null || SelectedCardButton.Card.AudioPath == null || File.Exists(SelectedCardButton.Card.AudioPath) == false) return;

            var audio = new Audio(cardBoard.SelectedCard.AudioPath);
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
            if (SelectedCardButton == null || SelectedCardButton.Card == null) return;

            var index = _cards.IndexOf(SelectedCardButton.Card);

            var card = new Card() {
                Title = SelectedCardButton.Card.Title,
                ImagePath = SelectedCardButton.Card.ImagePath,
                AudioPath = SelectedCardButton.Card.AudioPath,
                CardType = SelectedCardButton.Card.CardType
            };

            var cardEditorWindow = new CardEditorWindow(_settings, _tempDirPath, _yandexSpeech, card, WithoutSpace);
            cardEditorWindow.Owner = this;
            cardEditorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (cardEditorWindow.ShowDialog() != true) return;

            _cards[index].Title = cardEditorWindow.Caption;
            _cards[index].ImagePath = cardEditorWindow.ImagePath;
            _cards[index].AudioPath = cardEditorWindow.AudioPath;

            cardBoard.UpdateCard(index, _cards[index]);
        }

        private void RemoveCard(object sender, RoutedEventArgs e)
        {
            cardBoard.RemoveCard();
        }

        private void ChangeGridSize(object sender, RoutedEventArgs e)
        {
            var rows = Convert.ToInt32(rowsText.Text);
            var columns = Convert.ToInt32(columnsText.Text);

            if (rows != cardBoard.Rows) cardBoard.Rows = rows;

            if (columns != cardBoard.Columns) cardBoard.Columns = columns;
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
            cardBoard.MoveCardLeft();
        }

        private void MoveToRight(object sender, RoutedEventArgs e)
        {
            cardBoard.MoveCardRight();
        }

        private void MoveToDown(object sender, RoutedEventArgs e)
        {
            cardBoard.MoveCardDown();
        }

        private void MoveToUp(object sender, RoutedEventArgs e)
        {
            cardBoard.MoveCardUp();
        }

        private void SaveCardSet_Click(object sender, RoutedEventArgs e)
        {
            SaveCardSet();
        }

        private void SaveCardSetAs_Click(object sender, RoutedEventArgs e)
        {
            SaveCardSetAs();
        }

        private void GoToUserMode_Click(object sender, RoutedEventArgs e)
        {
            ChangeMode();
        }

        private bool SaveCardSet()
        {
            // Сохранить
            if (CurrentFileName != null && CurrentFileName != string.Empty)
            {
                return SaveCardSet(CurrentFileName);
            }
            else
            {
                return SaveCardSetAs();
            }
        }

        private bool SaveCardSet(string fileName)
        {
            try
            {
                var cardSetFile = new CardSetFile(cardBoard.Columns, cardBoard.Rows, WithoutSpace, _cards, Description);
                var cardSetLoader = new CardSetLoader();
                cardSetLoader.SaveToFile(fileName, cardSetFile);

                MessageBox.Show(this, "Набор успешно сохранен!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                CurrentFileName = fileName;

                IsEdited = false;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("При сохранении набора произошла ошибка! Подробнее: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        private bool SaveCardSetAs()
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "Linka files(*.linka)|*.linka";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

            return SaveCardSet(saveFileDialog.FileName);
        }

        private void LoadCardSet_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Linka files(*.linka)|*.linka";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            LoadCardSet(openFileDialog.FileName);
        }

        public void LoadCardSet(string path)
        {
            try
            {
                if (IsSave() == false) return; 

                var destPath = _tempDirPath + Guid.NewGuid() + "\\";

                var cardSetLoader = new CardSetLoader();
                var cardSetFile = cardSetLoader.LoadFromFile(path, destPath);

                cardBoard.Columns = cardSetFile.Columns;
                cardBoard.Rows = cardSetFile.Rows;
                Description = cardSetFile.Description??"";
                WithoutSpace = cardSetFile.WithoutSpace;

                _cards = cardSetFile.Cards;
                foreach (var card in _cards)
                {
                    if (card.ImagePath != null && card.ImagePath != string.Empty) card.ImagePath = destPath + card.ImagePath;
                    if (card.AudioPath != null && card.AudioPath != string.Empty) card.AudioPath = destPath + card.AudioPath;
                }
                cardBoard.Update(_cards);

                CurrentFileName = path;

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
            var cardEditorWindow = new CardEditorWindow(_settings, _tempDirPath, _yandexSpeech, WithoutSpace);
            cardEditorWindow.Owner = this;
            cardEditorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
         if (cardEditorWindow.ShowDialog() != true) return;

            var card = new Card(_cards.Count, cardEditorWindow.Caption, cardEditorWindow.ImagePath, cardEditorWindow.AudioPath, cardEditorWindow.CardType);
            _cards.Add(card);

            cardBoard.Update(_cards);
        }

        private void UpdatePageInfo()
        {
            pageInfoTextBlock.Text = string.Format("Текущая страница: {0} из {1}", cardBoard.CurrentPage + 1, cardBoard.CountPages);
        }

        private void ChangeStatusPlayButton()
        {
            if (SelectedCardButton == null || SelectedCardButton.Card == null) return;

            var card = SelectedCardButton.Card;

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
            e.Cancel = !IsSave();
        }

        public bool IsSave()
        {
            var result = true;

            if (IsEdited == true)
            {
                switch (MessageBox.Show(this, "Сохранить изменения?", "Сохранение", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                {
                    case MessageBoxResult.Yes:
                        {
                            // Сохраняем изменения и выходим
                            result = SaveCardSet();
                        }break;
                    case MessageBoxResult.No:
                        {
                            result = true;
                        }break;
                    default:
                        {
                            result = false;
                        }break;
                }

                if (result == true) IsEdited = false;
            }

            return result;
        }

        protected bool IsEdited { get; set; }

        public CardButton SelectedCardButton
        {
            get { return cardBoard.SelectedCardButton; }
        }

        public string CurrentFileName { get; set; }

        public Func<bool> ChangeMode;
        private Settings _settings;

        private void EditDescription_Click(object sender, RoutedEventArgs e)
        {
            var editor = new DescriptionWindow(true, Description);
            editor.Owner = this;
            if (editor.ShowDialog() != true) return;
            Description = editor.Description;
        }
    }
}
