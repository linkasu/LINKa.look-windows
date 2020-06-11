using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectInput;
using System.Windows.Threading;

namespace LinkaWPF
{
    public class Joysticks
    {
        private IList<Device> _devices;
        private Dictionary<Guid, byte[]> _buttons;        

        public event EventHandler<string> JoystickButtonDown;

        public Joysticks()
        {
            _devices = new List<Device>();
            _buttons = new Dictionary<Guid, byte[]>();

            UpdateDevices();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer;

            try
            {
                foreach (var device in _devices)
                {
                    var j = device.CurrentJoystickState;

                    // Состояние кнопок на предыдущей итерации
                    var prevButtons = _buttons[device.DeviceInformation.ProductGuid];

                    // Текущее состояние кнопок для устройства
                    var buttons = j.GetButtons();

                    for (var i = 0; i < buttons.Length; i++)
                    {
                        if ((buttons[i] != 0 && prevButtons == null) || (buttons[i] != 0 && prevButtons[i] != buttons[i]))
                        {
                            JoystickButtonDown?.Invoke(this, "J" + i);
                        }
                    }

                    _buttons[device.DeviceInformation.ProductGuid] = buttons;
                }
            }
            catch (Exception ex)
            {
                timer.Stop();
                UpdateDevices();
            }
        }

        private void UpdateDevices()
        {
            _devices.Clear();
            _buttons.Clear();

            foreach (DeviceInstance instance in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly))
            {
                // var productName = instance.ProductName;

                var device = new Device(instance.ProductGuid);

                // device.SetCooperativeLevel(null, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);

                device.Acquire();

                _devices.Add(device);
                _buttons.Add(device.DeviceInformation.ProductGuid, null);
            }

            var timer = new DispatcherTimer();

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();
        }

    }
}
