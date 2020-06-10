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

        public SettingsWindow(Settings settings)
        {
            InitializeComponent();

            Settings = settings;

            Update();

            _joysticks = new Joysticks();
            _joysticks.JoystickButtonDown += Joystick_JoystickButtonDown;
        }

        private void Joystick_JoystickButtonDown(object sender, int button)
        {
            var focusedElement = FocusManager.GetFocusedElement(grid);

            if (focusedElement == null) return;

            var actionName = (string)(focusedElement as TextBox).Tag;

            Settings.Keys["J" + button] = actionName;

            Update();
        }

        private void SetKey(object sender, KeyEventArgs e)
        {
            var actionName = (string)(sender as TextBox).Tag;

            Settings.Keys[e.Key.ToString()] = actionName;

            Update();
        }

        private void moveSelectorLeftTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var actionName = "MoveSelectorLeft";

            Settings.Keys[e.Key.ToString()] = actionName;

            Update();
        }

        private void moveSelectorRightTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var actionName = "MoveSelectorRight";

            Settings.Keys[e.Key.ToString()] = actionName;

            Update();
        }

        private void Update()
        {
            moveSelectorLeftTextBox.Text = "";
            moveSelectorRightTextBox.Text = "";
            moveSelectorUpTextBox.Text = "";
            moveSelectorDownTextBox.Text = "";

            foreach (var keyName in Settings.Keys)
            {
                switch (keyName.Value)
                {
                    case "MoveSelectorLeft":
                        {
                            if (moveSelectorLeftTextBox.Text == string.Empty)
                            {
                                moveSelectorLeftTextBox.Text = string.Format("{0};", keyName.Key);
                            }
                            else
                            {
                                moveSelectorLeftTextBox.Text += string.Format(" {0};", keyName.Key);
                            }
                        }break;
                    case "MoveSelectorRight":
                        {
                            if (moveSelectorRightTextBox.Text == string.Empty)
                            {
                                moveSelectorRightTextBox.Text = string.Format("{0};", keyName.Key);
                            }
                            else
                            {
                                moveSelectorRightTextBox.Text += string.Format(" {0};", keyName.Key);
                            }
                        }
                        break;
                    case "MoveSelectorUp":
                        {
                            if (moveSelectorUpTextBox.Text == string.Empty)
                            {
                                moveSelectorUpTextBox.Text = string.Format("{0};", keyName.Key);
                            }
                            else
                            {
                                moveSelectorUpTextBox.Text += string.Format(" {0};", keyName.Key);
                            }
                        }
                        break;
                    case "MoveSelectorDown":
                        {
                            if (moveSelectorDownTextBox.Text == string.Empty)
                            {
                                moveSelectorDownTextBox.Text = string.Format("{0};", keyName.Key);
                            }
                            else
                            {
                                moveSelectorDownTextBox.Text += string.Format(" {0};", keyName.Key);
                            }
                        }
                        break;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _joysticks.JoystickButtonDown -= Joystick_JoystickButtonDown;
        }
    }
}
