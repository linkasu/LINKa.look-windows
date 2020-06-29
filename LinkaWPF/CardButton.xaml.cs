using LinkaWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Tobii.Interaction.Wpf;

namespace LinkaWPF
{
    public partial class CardButton : AnimatedButton
    {
        public CardButton()
        {
            InitializeComponent();

            IsHasGaze = false;
            IsHasGazeEnabled = true;
            IsAnimatedClickEnabled = true;
        }

        public Card Card
        {
            get { return (Card)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        public event EventHandler<HasGazeChangedRoutedEventArgs> HasGazeChanged;

        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("Card", typeof(Card), typeof(CardButton), new PropertyMetadata(null, new PropertyChangedCallback(OnCardChanged)));

        private static void OnCardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var button = sender as CardButton;
            button.Card = (Card)args.NewValue;

            button.IsHasGaze = button.Card == null ? false : true;
        }

        protected override void OnHasGazeChanged(object sender, HasGazeChangedRoutedEventArgs e)
        {
            base.OnHasGazeChanged(sender, e);

            HasGazeChanged?.Invoke(sender, e);
        }
    }
}
