using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CyUSB;

namespace DDSControl
{
    /// <summary>
    /// Interface around the USBDevice provided by the CyUSBLibrary, 
    /// so that we can mock the components for unit testing.
    /// </summary>
    public interface IDDSMicrocontroller
    {
        void SendDataToEP1(byte[] Data);
        void ReceiveDataFromEP1(ref byte[] Data);
        void SendDataToEP2(byte[] Data);
        string Product { get; }
        string SerialNumber { get; }
    }
}
