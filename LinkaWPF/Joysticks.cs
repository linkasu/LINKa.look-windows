using System;
using System.Collections.Generic;
using Microsoft.DirectX.DirectInput;
using System.Windows.Threading;

namespace LinkaWPF
{
    public class Joysticks
    {
        private IList<Device> _devices;
        private Dictionary<Guid, byte[]> _buttons;
        private readonly int STRANGENUMBER = 32767;
        private int[] _axis;

        public event EventHandler<string> JoystickButtonDown;

        public Joysticks()
        {
            _devices = new List<Device>();
            _buttons = new Dictionary<Guid, byte[]>();
            _axis = new int[] { STRANGENUMBER, STRANGENUMBER };
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
                    var prevAxis = _axis;
                    _axis = new int[] { j.X, j.Y };

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
                    if (_axis[0] != prevAxis[0] && _axis[0] != STRANGENUMBER)
                    {
                        JoystickButtonDown?.Invoke(this, "JX" + (_axis[0] > STRANGENUMBER ? "R" : "L"));
                    }
                    if (_axis[1] != prevAxis[1] && _axis[1] != STRANGENUMBER)
                    {
                        JoystickButtonDown?.Invoke(this, "JY" + (_axis[1] > STRANGENUMBER ? "R" : "L"));
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
