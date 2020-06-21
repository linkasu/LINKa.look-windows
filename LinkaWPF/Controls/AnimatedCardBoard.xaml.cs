using CommandLine;
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
using Tobii.Interaction;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для AnimatedCardBoard.xaml
    /// </summary>
    public partial class AnimatedCardBoard : CardBoard
    {
        private AnimatedDelayedClick _delayedClick;

        private EyePositionStream _eyePositionStream;
        private Host _host;

        public AnimatedCardBoard()
        {
            InitializeComponent();

            _delayedClick = new AnimatedDelayedClick(3);
            _delayedClick.Ended += _delayedClick_Ended;
        }

        private void _eyePositionStream_Next(object sender, StreamData<EyePositionData> e)
        {
            if (e.Data.HasLeftEyePosition == false && e.Data.HasRightEyePosition == false)
            {
                Dispatcher.Invoke(() =>
                {
                    StopClick();
                });
            }
        }

        protected override void CardButton_Click(object sender, RoutedEventArgs e)
        {
            StopClick();

            base.CardButton_Click(sender, e);
        }

        protected override void CardButton_HazGazeChanged(object sender, RoutedEventArgs e)
        {
            var button = sender as CardButton;

            if (button.Card == null)
            {
                StopClick();
                return;
            }

            if (IsHazGazeEnabled == true) SelectCard(button);

            StartClick(button);

            base.CardButton_HazGazeChanged(sender, e);
        }

        private void _delayedClick_Ended(object sender, EventArgs e)
        {
            PressOnCardButton(sender);
        }

        private void StopClick()
        {
            _delayedClick.Stop();
        }

        private void StartClick(CardButton cardButton)
        {
            if (IsHazGazeEnabled == true && IsAnimatedClickEnabled == true)
            {
                _delayedClick.Start(cardButton);
            }
        }

        public Host Host
        {
            get { return _host; }
            set
            {
                _host = value;

                // Наблюдение за состоянием глаз
                _eyePositionStream = _host.Streams.CreateEyePositionStream();
                _eyePositionStream.Next += _eyePositionStream_Next;
            }
        }

        public bool IsHazGazeEnabled
        {
            get { return (bool)GetValue(IsHazGazeEnabledProperty); }
            set { SetValue(IsHazGazeEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsHazGazeEnabledProperty =
            DependencyProperty.Register("IsHazGazeEnabled", typeof(bool), typeof(AnimatedCardBoard), new PropertyMetadata(true, new PropertyChangedCallback(IsHazGazeEnabledChanged)));

        private static void IsHazGazeEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var cardBoard = sender as AnimatedCardBoard;
            cardBoard.IsHazGazeEnabled = (bool)args.NewValue;
            if (cardBoard.IsHazGazeEnabled == false)
            {
                cardBoard.StopClick();
            }
        }

        public bool IsAnimatedClickEnabled
        {
            get { return (bool)GetValue(IsAnimatedClickEnabledProperty); }
            set { SetValue(IsAnimatedClickEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsAnimatedClickEnabledProperty =
            DependencyProperty.Register("IsAnimatedProperty", typeof(bool), typeof(AnimatedCardBoard), new PropertyMetadata(true, new PropertyChangedCallback(IsAnimatedClickEnabledChanged)));

        private static void IsAnimatedClickEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var cardBoard = sender as AnimatedCardBoard;
            cardBoard.IsAnimatedClickEnabled = (bool)args.NewValue;
            if (cardBoard.IsAnimatedClickEnabled == false)
            {
                cardBoard.StopClick();
            }
        }
    }
}
