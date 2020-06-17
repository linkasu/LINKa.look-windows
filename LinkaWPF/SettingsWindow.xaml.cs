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

        private Dictionary<string, int> _actionDictionary;

        private Settings _settings;

        public SettingsWindow(Settings settings)
        {
            InitializeComponent();

            _settings = settings;

            InitActions();

            _joysticks = new Joysticks();
            _joysticks.JoystickButtonDown += Joystick_JoystickButtonDown;
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
            var focusedElement = FocusManager.GetFocusedElement(grid);

            if (focusedElement == null) return;

            _settings.Keys[keyName] = (string)(focusedElement as TextBox).Tag;

            Update();
        }

        private void InitActions()
        {
            _actionDictionary = new Dictionary<string, int>();
            _actionList = new List<ActionItem>();

            AddAction("MoveSelectorLeft", "Селектор влево");
            AddAction("MoveSelectorRight", "Селектор вправо");
            AddAction("MoveSelectorUp", "Селектор вверх");
            AddAction("MoveSelectorDown", "Селектор вниз");

            _settings.Keys["J"] = "MoveSelectorLeft";

            foreach (var key in _settings.Keys)
            {
                var index = _actionDictionary[key.Value];
                _actionList[index].Keys.Add(key.Key);
            }

            actionItems.ItemsSource = _actionList;

            Update();
        }

        private void AddAction(string name, string title)
        {
            _actionList.Add(new ActionItem() { Name = name, Title = title, Keys = new List<string>() });
            _actionDictionary.Add(name, _actionList.Count - 1);
        }

        private void Update()
        {
            actionItems.ItemsSource = null;
            actionItems.ItemsSource = _actionList;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _joysticks.JoystickButtonDown -= Joystick_JoystickButtonDown;
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Settings = _settings;
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Settings = null;
            DialogResult = false;
        }

        public Settings Settings { get; private set; }
    }
}
