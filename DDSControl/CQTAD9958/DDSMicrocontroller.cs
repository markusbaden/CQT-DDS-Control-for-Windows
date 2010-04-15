using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CyUSB;

namespace DDSControl
{
    /// <summary>
    /// Implements the ICQTUSBDevice interface and is thus a representation
    /// of the USB DDS that can be mocked.
    /// </summary>
    public class DDSMicrocontroller : IDDSMicrocontroller
    {
        private CyUSBDevice cyUSBDevice;
        private CyUSBEndPoint EP1OUT;
        private CyUSBEndPoint EP1IN;
        private CyUSBEndPoint EP2OUT;
        
        public DDSMicrocontroller(CyUSBDevice CyUSBDevice)
        {
            cyUSBDevice = CyUSBDevice;
            // Switch to right interface
            cyUSBDevice.AltIntfc = 1;
            EP1OUT = cyUSBDevice.EndPointOf(0x01);
            EP1IN = cyUSBDevice.EndPointOf(0x81);
            // Define Endpoints
            EP2OUT = cyUSBDevice.EndPointOf(0x02);
        }

        #region ICQTUSBDevice Members

        public string SerialNumber
        {
            get { return cyUSBDevice.SerialNumber; }
        }
        
        public string Product
        {
            get { return cyUSBDevice.Product; }
        }

        public void SendDataToEP1(byte[] Data)
        {
            int length = Data.Length;
            EP1OUT.XferData(ref Data, ref length);
        }

        public void ReceiveDataFromEP1(ref byte[] Data)
        {
            int length = Data.Length;
            EP1IN.XferData(ref Data, ref length);
        }

        public void SendDataToEP2(byte[] Data)
        {
            int length = Data.Length;
            EP2OUT.XferData(ref Data, ref length);
        }

        #endregion
    }
}
