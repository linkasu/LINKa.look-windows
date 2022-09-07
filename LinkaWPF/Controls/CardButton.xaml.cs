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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tobii.Interaction.Wpf;
using WpfAnimatedGif;

namespace LinkaWPF
{
    public partial class CardButton : AnimatedButton
    {
        static List<CardButton> buttons = new List<CardButton>();
        public CardButton()
        {
            InitializeComponent();
            IsHasGaze = false;
            buttons.Add(this);
        }
        public static void StopAll()
        {
            foreach (var item in buttons)
            {
                if (item != null) item.Stop();
            }
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


        public void Stop()
        {
            if (ImageField == null) return;

            ImageBehavior.SetRepeatBehavior(ImageField, new RepeatBehavior(1));
        }
        public void Play()
        {
            if (ImageField == null) return;
            ImageBehavior.SetRepeatBehavior(ImageField, RepeatBehavior.Forever);

        }
    }
}