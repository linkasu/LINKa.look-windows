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

            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(3));
            animation.Completed += new EventHandler((sender, e) => {
                _progress.Visibility = Visibility.Hidden;
                OnClick();
            });
            Storyboard.SetTarget(animation, _progress);
            Storyboard.SetTargetProperty(animation, new PropertyPath(CircularProgressBar.PercentageProperty));

            _sb = new Storyboard();
            _sb.Children.Add(animation);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _grid = this.Template.FindName("grid", this) as Grid;
            _grid.Children.Add(_progress);
        }
    }
}
