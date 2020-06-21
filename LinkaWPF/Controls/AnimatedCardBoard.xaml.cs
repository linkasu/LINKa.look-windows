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
            if (GazeEnable == true && IsAnimatedClick == true)
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

        public bool GazeEnable
        {
            get { return (bool)GetValue(GazeEnableProperty); }
            set { SetValue(GazeEnableProperty, value); }
        }

        public static readonly DependencyProperty GazeEnableProperty =
            DependencyProperty.Register("GazeEnable", typeof(bool), typeof(AnimatedCardBoard), new PropertyMetadata(true, new PropertyChangedCallback(OnGazeEnableChanged)));

        private static void OnGazeEnableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var cardBoard = sender as AnimatedCardBoard;
            cardBoard.GazeEnable = (bool)args.NewValue;
            if (cardBoard.GazeEnable == false)
            {
                cardBoard.StopClick();
            }
        }

        public bool IsAnimatedClick
        {
            get { return (bool)GetValue(IsAnimatedClickProperty); }
            set { SetValue(IsAnimatedClickProperty, value); }
        }

        public static readonly DependencyProperty IsAnimatedClickProperty =
            DependencyProperty.Register("IsAnimatedProperty", typeof(bool), typeof(AnimatedCardBoard), new PropertyMetadata(true, new PropertyChangedCallback(IsAnimatedClickChanged)));

        private static void IsAnimatedClickChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var cardBoard = sender as AnimatedCardBoard;
            cardBoard.IsAnimatedClick = (bool)args.NewValue;
            if (cardBoard.IsAnimatedClick == false)
            {
                cardBoard.StopClick();
            }
        }
    }
}
