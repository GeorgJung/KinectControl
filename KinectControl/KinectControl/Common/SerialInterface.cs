/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KinectControl.Common
{
    public class SerialInterface
    {
    CommunicationManager comm = new CommunicationManager();
    string transType, cboBaud, cboData, cboParity, cboPort, cboStop,cmdClose  = string.Empty;
    
        public void Initialize()
        {
            LoadValues();
            SetDefaults();
        }

        /// <summary>
        /// Method to initialize serial port
        /// values to standard defaults
        /// </summary>
        private void SetDefaults()
        {
            cboPort = "0";
            cboBaud = "9600";
            cboParity = "0";
            cboStop = "0";
            cboData = "0";
        }

        /// <summary>
        /// methos to load our serial
        /// port option values
        /// </summary>
        private void LoadValues()
        {
            comm.SetPortNameValues(cboPort);
            comm.SetParityValues(cboParity);
            comm.SetStopBitValues(cboStop);
        }

        private void OpenPort()
        {
            comm.Parity = cboParity;
            comm.StopBits = cboStop;
            comm.DataBits = cboData;
            comm.BaudRate = cboBaud;
            comm.PortName = cboPort;
            comm.OpenPort();
        }

        public void sendData(string text)
        {
            comm.WriteData(text);
        }
        
        private void ClosePort()
        {
            comm.PortName = cboPort;
            comm.ClosePort();            
        }
    }
}*/