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
        public Settings Settings { get; }
        private Joysticks _joysticks;

        private IList<ActionItem> _actionList;

        public SettingsWindow(Settings settings)
        {
            InitializeComponent();

            InitActions();

            Settings = settings;

            Update();

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

            Settings.Keys[keyName] = (string)(focusedElement as TextBox).Tag;

            Update();
        }

        private void InitActions()
        {
            _actionList = new List<ActionItem>();
            _actionList.Add(new ActionItem() { Name = "MoveSelectorLeft", NameRU = "Селектор влево", Keys = new List<string>() });
            _actionList.Add(new ActionItem() { Name = "MoveSelectorRight", NameRU = "Селектор вправо", Keys = new List<string>() });
            _actionList.Add(new ActionItem() { Name = "MoveSelectorUp", NameRU = "Селектор вверх", Keys = new List<string>() });
            _actionList.Add(new ActionItem() { Name = "MoveSelectorDown", NameRU = "Селектор вниз", Keys = new List<string>() });

            actionItems.ItemsSource = _actionList;
        }

        private void Update()
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _joysticks.JoystickButtonDown -= Joystick_JoystickButtonDown;
        }
    }
}
