using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CyUSB;

namespace DDSControl
{
    /// <summary>
    /// Interface abstracting the functionality provided by Christian Kurtsiefer's firmware.
    /// The EZ USB Chip is called via the CyUSB.dll provided by Cypress. This interface makes
    /// calls to the microcontroller unit testable.
    /// </summary>
    public interface IDDSUSBChip
    {
        #region Functions for directly communicating with the endpoints
        
        void SendDataToEP1(byte[] Data);
        void ReceiveDataFromEP1(ref byte[] Data);
        void SendDataToEP2(byte[] Data);
        
        #endregion

        #region Functions corresponding to the EP1 commands

        void Full_DDS_Reset();

        #endregion

        #region Information provided via USB

        string Product { get; }
        string SerialNumber { get; }

        #endregion

    }
}
