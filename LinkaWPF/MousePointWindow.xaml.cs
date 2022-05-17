using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using Tobii.Interaction;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для MousePointWindow.xaml
    /// </summary>
    public partial class MousePointWindow : Window
    {
        private Host host;
        double Percentage
        {
            get
            {
                return Circlebar.Percentage;
            }
            set
            {

                Application
                .Current
                .Dispatcher
                .Invoke(new Action(() =>
                {
                    Circlebar.Percentage = value;
                }));
            }
        }
        private List<Vector> points = new List<Vector>();
        private int size = 15;
        private Vector? WatchPoint;
        private double? StartTS;
        private double lastTS = 0;
        private GazePointDataStream Stream;

        private Settings settings;
        private double ReactionFilter {
            get
            {
                return settings.MousePointReactionFilter;
            }
        }
        private double OutFilter = 15;
        private double Timeout {
            get
            {
                return settings.ClickDelay*1000;
            }
        }

        public MousePointWindow(Tobii.Interaction.Host host, Settings settings)
        {
            InitializeComponent();

            this.host = host;
            this.settings = settings;
            Left = 0;
            Top = 0;
            Stream = host
                .Streams
                .CreateGazePointDataStream(Tobii.Interaction.Framework.GazePointDataMode.LightlyFiltered);
                Stream.GazePoint(GazePoint);

        }

        private void GazePoint(double x, double y, double ts)
        {
            double delta = ts - lastTS;
            if ((lastTS != 0) && delta > 1000)
            {
                StopWatchPoint();
            }
            lastTS = ts;

            Vector point = new Vector(x, y);
            points.Add(point);
            if (points.Count > size)
            {
                points.RemoveAt(0);
            }
            if (points.Count < size)
            {
                return;
            }
            var middle = MousePointCalcs.CalcMiddleLength(points);

            if (!WatchPoint.HasValue)
            {

                if (middle < ReactionFilter)
                {
                    StartWatchPoint(MousePointCalcs.MiddleValue( points), ts);
                }
            }
            else
            {
                if (middle > OutFilter)
                {
                    StopWatchPoint();
                }
                else
                {
                    double watchTime = ts - StartTS.Value;
                    Percentage = (watchTime/Timeout)* 100;
                    if (watchTime >= Timeout)
                    {
                        Click();
                        StopWatchPoint();
                    }
                }
            }

        }

        private void Click()
        {
            Application
            .Current
            .Dispatcher
            .Invoke(new Action(() =>
            {
                MouseOperations.SetCursorPosition((int)WatchPoint.Value.X, (int)WatchPoint.Value.Y);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            }));
        }

        private void SetWindowPosition(Vector point)
        {
            Application
            .Current
            .Dispatcher
            .Invoke(new Action(() =>
            {

                Left = point.X - Width / 2;
                Top = point.Y - Height / 2;
            }));
        }

        private void StartWatchPoint(Vector point, double ts)
        {
            WatchPoint = point;
            Percentage = 0;
            StartTS = ts;
            SetWindowPosition(WatchPoint.Value);
        }

        private void StopWatchPoint()
        {
            WatchPoint = null;
            StartTS = null;
            points.Clear();
            Percentage = 0;
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Stream.IsEnabled = false;
        }

    }
}
