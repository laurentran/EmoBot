using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotKit;
using System.Diagnostics;
using Emo.Common;

namespace EmoBot
{
    public class SpheroController
    {
        private Sphero _robot;
        private DateTime _lastChangeTime = DateTime.MinValue;

        public void Connect()
        {
            SetupRobotConnection();
        }

        public void Disconnect()
        {
            ShutdownRobotConnection();
        }

        private bool _colorLock = false;

        public void ChangeColor(SpheroColor color)
        {
            var currentTime = DateTime.Now;
            var deltaTime = currentTime - _lastChangeTime;
            if (deltaTime.TotalMilliseconds > 100)
            {
                _colorLock = false;
                _lastChangeTime = currentTime;
                //_robot.SetRGBLED(255, 255, 255);
            }

            if (_colorLock) return;

            if (color == SpheroColor.RED)
            {
                _robot.SetRGBLED(255, 0, 0);
                _lastChangeTime = currentTime;
                _colorLock = true;
            }
            else if (color == SpheroColor.GREEN)
            {
                _robot.SetRGBLED(0, 255, 0);
                _lastChangeTime = currentTime;
                _colorLock = true;
            }
            else
            {
                //_robot.SetRGBLED(255, 255, 255);
                _colorLock = false; 
            }
        }


        private void SetupRobotConnection()
        {
            var provider = RobotProvider.GetSharedProvider();
            provider.DiscoveredRobotEvent += OnRobotDiscovered;
            provider.NoRobotsEvent += OnNoRobotsEvent;
            provider.ConnectedRobotEvent += OnRobotConnected;
            provider.FindRobots();
        }

        private void OnRobotDiscovered(object sender, Robot robot)
        {
            Debug.WriteLine(string.Format("Discovered \"{0}\"", robot.BluetoothName));

            if (_robot == null)
            {
                var provider = RobotProvider.GetSharedProvider();
                provider.ConnectRobot(robot);
                _robot = (Sphero)robot;
            }
        }

        private void OnNoRobotsEvent(object sender, EventArgs e)
        {
            Debug.WriteLine("Cannot find any robots");
        }


        private void OnRobotConnected(object sender, Robot robot)
        {
            Debug.WriteLine(string.Format("Connected to {0}", robot));

            _robot.SetRGBLED(255, 255, 255);
        }



        private void ShutdownRobotConnection()
        {
            if (_robot != null)
            {
                _robot.SensorControl.StopAll();
                _robot.Sleep();

                RobotProvider provider = RobotProvider.GetSharedProvider();
                provider.DiscoveredRobotEvent -= OnRobotDiscovered;
                provider.NoRobotsEvent -= OnNoRobotsEvent;
                provider.ConnectedRobotEvent -= OnRobotConnected;
            }
        }
    }
}
