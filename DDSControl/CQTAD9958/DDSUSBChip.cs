using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CyUSB;

namespace DDSControl
{
    /// <summary>
    /// Class implementing the functionality provided by Christian Kurtsiefers firmware on the CQT DDS board.
    /// All interaction with the EZ USB Chip is done via the CyUSB.dll provided by Cypress.
    /// </summary>
    public class DDSUSBChip : IDDSUSBChip
    {
        private CyUSBDevice cyUSBDevice;
        private CyUSBEndPoint EP1OUT;
        private CyUSBEndPoint EP1IN;
        private CyUSBEndPoint EP2OUT;
        
        public DDSUSBChip(CyUSBDevice CyUSBDevice)
        {
            cyUSBDevice = CyUSBDevice;
            // Switch to interface 1  which provides several Endpoints
            cyUSBDevice.AltIntfc = 1;
            // Define Endpoints
            EP1OUT = cyUSBDevice.EndPointOf(0x01);
            EP1IN = cyUSBDevice.EndPointOf(0x81);
            EP2OUT = cyUSBDevice.EndPointOf(0x02);
        }

        #region ICQTUSBDevice Members

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

        public void Full_DDS_Reset()
        {

        }

        public string SerialNumber
        {
            get { return cyUSBDevice.SerialNumber; }
        }

        public string Product
        {
            get { return cyUSBDevice.Product; }
        }

        #endregion
    }
}
