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
    /// Логика взаимодействия для CardButton.xaml
    /// </summary>
    public partial class CardButton : UserControl
    {
        private bool _isMouseDown;
        public CardButton()
        {
            InitializeComponent();
        }

        public Models.Card Card
        {
            get { return (Models.Card)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("Card", typeof(Models.Card), typeof(CardButton), new PropertyMetadata(null, new PropertyChangedCallback(OnCardChanged)));

        private static void OnCardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CardButton button = sender as CardButton;
            button.Card = (Models.Card)args.NewValue;
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CardButton));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMouseDown == false) return;

            RoutedEventArgs newEventArgs = new RoutedEventArgs(CardButton.ClickEvent);
            RaiseEvent(newEventArgs);

            _isMouseDown = false;
        }

        private void OnHasGazeChanged(object sender, Tobii.Interaction.Wpf.HasGazeChangedRoutedEventArgs e)
        {
            
        }
    }
}
