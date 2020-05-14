using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace LinkaWPF
{
    class AnimatedDelayedClick
    {
        private CircularProgressBar _progress;
        private Storyboard _sb;
        private object _parent;

        public AnimatedDelayedClick(double delay)
        {
            _progress = new CircularProgressBar();
            _progress.StrokeThickness = 6;
            _progress.HorizontalAlignment = HorizontalAlignment.Center;
            _progress.VerticalAlignment = VerticalAlignment.Center;
            _progress.Visibility = Visibility.Hidden;

            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(delay));
            animation.Completed += new EventHandler((sender, e) => {
                _progress.Visibility = Visibility.Hidden;
                Ended(_parent, new EventArgs());
            });
            Storyboard.SetTarget(animation, _progress);
            Storyboard.SetTargetProperty(animation, new PropertyPath(CircularProgressBar.PercentageProperty));

            _sb = new Storyboard();
            _sb.Children.Add(animation);
        }

        public void Start(CardButton cardButton)
        {
            if (_parent != null)
            {
                (_parent as CardButton).RemoveElement(_progress);
            }

            // Добавляем прогресс на карточку
            cardButton.AddElement(_progress);

            _parent = cardButton;

            _progress.Radius = Convert.ToInt32((cardButton.ActualHeight - 20) / 2);
            _progress.Visibility = Visibility.Visible;

            _sb.Stop();
            _sb.Begin();
        }

        public void Stop()
        {
            _progress.Visibility = Visibility.Hidden;
        }

        public event EventHandler Ended;
    }
}
