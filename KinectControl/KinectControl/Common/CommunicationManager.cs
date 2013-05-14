using System;
using System.IO.Ports;
using System.Diagnostics;
namespace KinectControl.Common
{
    class CommunicationManager
    {
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };
        private string _baudRate = string.Empty;
        private string _portName = string.Empty;
        private static SerialPort port1;
        public string BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }

        public CommunicationManager(string baud, string name)
        {
            _baudRate = baud;
            _portName = name;
            port1 = new SerialPort(name, Int32.Parse(baud));
            port1.DataReceived += new SerialDataReceivedEventHandler(port1_DataReceived);
        }

        public void WriteData(string msg)
        {
            if (!(port1.IsOpen == true)) port1.Open();
            port1.Write(msg);
        }

        public bool OpenPort()
        {
            try
            {
                port1.Open();
                Debug.WriteLine(MessageType.Normal, "Port opened at " + DateTime.Now + "\n");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MessageType.Error, ex.Message);
                return false;
            }
        }

        public bool ClosePort()
        {
            try
            {
                if ((port1.IsOpen == true) && (port1.BytesToRead == 0)) port1.Close();
                Debug.WriteLine(MessageType.Normal, "Port closed at " + DateTime.Now + "\n");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MessageType.Error, ex.Message);
                return true;
            }
        }
        //public void ChoosePort()
        //{
        //    foreach (string portname in SerialPort.GetPortNames())
        //    {
        //        try
        //        {
        //            _portName = "COM18";
        //            OpenPort();
        //            port1.WriteLine("1");
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex.Message);
        //        }

        //    }
        //}

        void port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string msg = port1.ReadExisting();
            Debug.WriteLine(MessageType.Incoming, msg + "\n");
        }
    }
}


