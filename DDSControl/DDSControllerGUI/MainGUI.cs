using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CyUSB;

namespace DDSControl
{
    public partial class MainGUI : Form
    {

        #region log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion //log4net

        private USBDeviceList deviceList;
        private List<AD9958> ddsList;

        public MainGUI()
        {
            InitializeComponent();
            initializeDeviceList();
            refreshDeviceListBox();
            
            // Select first DDS in list, this will also 
            // initialize currentDDS. This is convenient
            // for if one only wants to control a single
            // DDS.
            if (deviceListBox.Items.Count > 0)
                deviceListBox.SelectedIndex = 0;
        }
        
        private void initializeDeviceList()
        {
            // Chose all USB Devices with the generic CyUSB driver
            // The argument of the constructor is a mask saying
            // just that.
            deviceList = new USBDeviceList(CyConst.DEVICES_CYUSB);
            ddsList = new List<AD9958>();

            deviceList.DeviceAttached += new EventHandler(deviceAttached);
            deviceList.DeviceRemoved += new EventHandler(deviceRemoved);
        }

        private void deviceAttached(object sender, EventArgs e)
        {
            refreshDeviceListBox();
        }

        private void deviceRemoved(object sender, EventArgs e)
        {
            refreshDeviceListBox();
        }

        private void refreshDeviceListBox()
        {
            deviceListBox.Items.Clear();
            foreach (USBDevice dev in deviceList)
            {
                deviceListBox.Items.Add(String.Format("{0} / {1}", dev.Product, dev.SerialNumber));
                ddsList.Add(new AD9958(new DDSMicrocontroller(dev as CyUSBDevice)));
            }
        }

        private void setChannel(AD9958 dds, int ChannelNumber, Dictionary<string, double> Values)
        {
            if (ChannelNumber == 2)
            {
                dds.SetTwoChannelRelativePhase(Values["frequency"], Values["relativephase"]);
            }
            else
            {
                #warning DDS Control GUI performs some functionality of the AD9958 class when setting channels
                dds.SelectChannelToWrite(ChannelNumber);
                dds.SetFrequency(Values["frequency"]);
                dds.SetAmplitude((int)Values["amplitude"]);
            }
        }
            

        // According to the CyUSB help USBDeviceList implements the IDisposable
        // interface and thus we should call the following function to dispose
        // it
        private void MainGUI_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (deviceList != null) deviceList.Dispose();
        }

        private void fullResetButton_Click(object sender, EventArgs e)
        {
            AD9958 selectedDDS = ddsList[deviceListBox.SelectedIndex];
            selectedDDS.FullReset();

            selectedDDS.SetMode("singletone");
            selectedDDS.SetLevels(2);              
        }

        private void setChannelButton_Click(object sender, EventArgs e)
        {
            // Parse information
            AD9958 selectedDDS = ddsList[deviceListBox.SelectedIndex];
            int selectedChannel = channelTabControl.SelectedIndex;
            Dictionary<string, double> values = ((IExtractValues)channelTabControl.SelectedTab.Controls[0]).ExtractValues();

            // Set channel
            setChannel(selectedDDS, selectedChannel, values);
        }
    }
}
