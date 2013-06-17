using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectControl.Common
{
    public class Device
    {
        private string name;
        private bool isSwitchedOn;
        public bool IsSwitchedOn
        {
            get { return isSwitchedOn; }
            set { isSwitchedOn = value; }
        }
        private string switchOnSignal;
        private string switchOffSignal;

        public Device()
        {
            name = "";
            switchOnSignal = "1";
            switchOffSignal = "0";
            isSwitchedOn = false;
        }
        public Device(string on, string off)
        {
            name = "";
            switchOnSignal =on;
            switchOffSignal =off;
            isSwitchedOn = false;
        }
        public Device(string name, string switchOnSignal, string switchOffSignal)
        {
            this.name = name;
            this.switchOnSignal = switchOnSignal;
            this.switchOffSignal = switchOffSignal;
            isSwitchedOn = false;
        }
        public void switchOn(CommunicationManager comm)
        {
            comm.OpenPort();
            comm.WriteData(switchOnSignal);
            comm.ClosePort();
            isSwitchedOn = true;
        }
        public void switchOff(CommunicationManager comm)
        {
            comm.OpenPort();
            comm.WriteData(switchOffSignal);
            isSwitchedOn = false;
            comm.ClosePort();
        }
    }
}
