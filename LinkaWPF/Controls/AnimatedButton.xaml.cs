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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tobii.Interaction.Wpf;

namespace LinkaWPF
{
    public partial class AnimatedButton : Button
    {
        private CircularProgressBar _progress;
        private Storyboard _sb;
        private Grid _grid;
        private Brush _prevBackground;
        private DoubleAnimation _animation;

        public AnimatedButton()
        {
            InitializeComponent();

            _prevBackground = Background;

            // Behaviors.SetIsGazeAware(this, true);
            // Behaviors.SetIsActivatable(this, true);
            // Behaviors.SetGazeAwareDelayTime(this, 200);
            // Behaviors.SetGazeAwareMode(this, Tobii.Interaction.Framework.GazeAwareMode.Normal);
            // Behaviors.SetIsTentativeFocusEnabled(this, true);

            Behaviors.SetIsGazeAware(this, true);
            Behaviors.AddHasGazeChangedHandler(this, (sender, e) =>
            {
                var button = sender as Button;

                if (e.HasGaze == true)
                {
                    Background = Brushes.Yellow;
                    _progress.Visibility = Visibility.Visible;
                    _progress.Radius = Convert.ToInt32((ActualHeight - 20) / 2);
                    _sb.Begin();
                }
                else
                {
                    Background = _prevBackground;
                    _progress.Visibility = Visibility.Hidden;
                    _sb.Stop();
                }
            });

            _progress = new CircularProgressBar()
            {
                StrokeThickness = 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Hidden
            };

            _animation = new DoubleAnimation();
            _animation.From = 0;
            _animation.To = 100;
            _animation.Duration = TimeSpan.FromSeconds(3);
            _animation.Completed += new EventHandler((sender, e) => {
                _progress.Visibility = Visibility.Hidden;
                OnClick();
            });
            Storyboard.SetTarget(_animation, _progress);
            Storyboard.SetTargetProperty(_animation, new PropertyPath(CircularProgressBar.PercentageProperty));

            _sb = new Storyboard();
            _sb.Children.Add(_animation);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _grid = this.Template.FindName("grid", this) as Grid;
            _grid.Children.Add(_progress);
        }

        public double ClickDelay
        {
            get { return (double)GetValue(ClickDelayProperty); }
            set { SetValue(ClickDelayProperty, value); }
        }

        public static readonly DependencyProperty ClickDelayProperty =
            DependencyProperty.Register("ClickDelay",
                typeof(double), typeof(AnimatedButton),
                new PropertyMetadata((double)3, new PropertyChangedCallback(ClickDelayChanged)));

        private static void ClickDelayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var button = sender as AnimatedButton;
            button._animation.Duration = TimeSpan.FromSeconds((double)args.NewValue);
        }
    }
}
