using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CyUSB;

namespace DDSControl
{
    public class CyLabviewHelper
    {
        private int i;

        public CyLabviewHelper()
        {
            i = 0;
        }
        
        public List<string> ConnectedDDSBoards()
        {
            List<string> connectedDDSBoards = new List<string>();
            USBDeviceList deviceList = new USBDeviceList(CyConst.DEVICES_CYUSB);

            foreach (USBDevice device in deviceList)
            {
                connectedDDSBoards.Add(device.SerialNumber);
            }
            return connectedDDSBoards;
        }
    }
}
