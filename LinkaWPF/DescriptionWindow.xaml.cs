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
    /// Логика взаимодействия для DescriptionWindow.xaml
    /// </summary>
    public partial class DescriptionWindow : Window
    {
        public DescriptionWindow(bool editor, string description)
        {
            InitializeComponent();
            Editor = editor;
            Description = description;
            EditField.Text = description;
            EditField.Focusable = editor;
            WrapButtons.Visibility = editor ? Visibility.Visible : Visibility.Hidden;
        }

        public bool Editor { get; }
        public string Description { get; private set; }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Description = EditField.Text;
            Close();
        }
    }
}
