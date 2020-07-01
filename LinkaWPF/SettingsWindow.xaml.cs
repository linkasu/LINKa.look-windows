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
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Joysticks _joysticks;

        private IList<ActionItem> _actionList;

        private TextBlock _focusedElement;

        private Dictionary<string, int> _actionDictionary;

        private Settings _settings;

        public SettingsWindow(Settings settings)
        {
            InitializeComponent();

            _settings = settings.Clone() as Settings;

            InitActions();

            _joysticks = new Joysticks();
            _joysticks.JoystickButtonDown += Joystick_JoystickButtonDown;

            isHazGazeEnabledCheckBox.IsChecked = _settings.IsHasGazeEnabled;
            isAnimatedClickEnabledCheckBox.IsChecked = _settings.IsAnimatedClickEnabled;
            isPlayAudioFromCardCheckBox.IsChecked = _settings.IsPlayAudioFromCard;
            isPageButtonVisibleCheckBox.IsChecked = _settings.IsPageButtonVisible;

            isJoystickEnabledCheckBox.IsChecked = _settings.IsJoystickEnabled;
            isKeyboardEnabledCheckBox.IsChecked = _settings.IsKeyboardEnabled;
            isMouseEnabledCheckBox.IsChecked = _settings.IsMouseEnabled;

            DataContext = _settings;
        }

        #region Events

        private void Joystick_JoystickButtonDown(object sender, string buttonName)
        {
            SetKeyName(buttonName);
        }

        private void SetKey(object sender, KeyEventArgs e)
        {
            SetKeyName(e.Key.ToString());
        }
        #endregion

        private void SetKeyName(string keyName)
        {
            if (_focusedElement == null) return;

            var actionName = (string)_focusedElement.Tag;

            var actionItem = GetActionItemFromName(actionName);

            if (actionItem == null) return;

            foreach (var item in _actionList)
            {
                var result = item.Keys.Remove(keyName);
            }

            actionItem.Keys.Add(keyName);
        }

        private void InitActions()
        {
            _actionDictionary = new Dictionary<string, int>();
            _actionList = new List<ActionItem>();

            AddAction("MoveSelectorLeft", "Селектор влево");
            AddAction("MoveSelectorRight", "Селектор вправо");
            AddAction("MoveSelectorUp", "Селектор вверх");
            AddAction("MoveSelectorDown", "Селектор вниз");
            AddAction("Enter", "Выбор карточки");

            foreach (var keyItem in _settings.Keys)
            {
                var actionItem = GetActionItemFromName(keyItem.Value);

                if (actionItem == null) continue;

                actionItem.Keys.Add(keyItem.Key);
            }

            actionItems.ItemsSource = _actionList;
        }

        private void AddAction(string name, string title)
        {
            _actionList.Add(new ActionItem(name, title));
            _actionDictionary.Add(name, _actionList.Count - 1);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _joysticks.JoystickButtonDown -= Joystick_JoystickButtonDown;
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            _settings.Keys.Clear();
            foreach (var actionName in _actionDictionary)
            {
                var actionItem = GetActionItemFromName(actionName.Key);
                foreach (var keyName in actionItem.Keys)
                {
                    _settings.Keys[keyName] = actionName.Key;
                }
            }

            _settings.IsHasGazeEnabled = isHazGazeEnabledCheckBox.IsChecked ?? false;
            _settings.IsAnimatedClickEnabled = isAnimatedClickEnabledCheckBox.IsChecked ?? false;
            _settings.IsPlayAudioFromCard = isPlayAudioFromCardCheckBox.IsChecked ?? false;
            _settings.IsPageButtonVisible = isPageButtonVisibleCheckBox.IsChecked ?? true;

            _settings.IsJoystickEnabled = isJoystickEnabledCheckBox.IsChecked ?? true;
            _settings.IsKeyboardEnabled = isKeyboardEnabledCheckBox.IsChecked ?? true;
            _settings.IsMouseEnabled = isMouseEnabledCheckBox.IsChecked ?? true;

            Settings = _settings;
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Settings = null;
            DialogResult = false;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_focusedElement != null) _focusedElement.Background = Brushes.White;

            _focusedElement = sender as TextBlock;
            _focusedElement.Background = Brushes.Orange;
        }

        private ActionItem GetActionItemFromName(string actionName)
        {
            int index = 0;

            if (_actionDictionary.TryGetValue(actionName, out index) == false) return null;

            return _actionList[index];
        }

        public Settings Settings { get; private set; }

        private void RemoveLastKey(object sender, RoutedEventArgs e)
        {
            var actionName = (string)(sender as Button).Tag;

            var actionItem = GetActionItemFromName(actionName);

            if (actionItem == null || actionItem.Keys.Count == 0) return;

            actionItem.Keys.RemoveAt(actionItem.Keys.Count - 1);
        }

        private void decreaseButton_Click(object sender, RoutedEventArgs e)
        {
            _settings.ClickDelay -= 0.1;

            if (_settings.ClickDelay < 0.5) _settings.ClickDelay = 0.5;
        }

        private void increaseButton_Click(object sender, RoutedEventArgs e)
        {
            _settings.ClickDelay += 0.1;
        }
    }
}
