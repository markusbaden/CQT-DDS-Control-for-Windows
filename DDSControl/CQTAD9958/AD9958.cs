using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CyUSB;

namespace DDSControl
{
    public class AD9958 : DDSChip
    {
        #region log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion //log4net

        #region DDSChip members
        
        /// <summary>
        /// Description of the device, i.e. the product information provided via USB.
        /// </summary>
        public override string Description
        {
            get { return device.Product; }
        }
        
        /// <summary>
        /// Serial number of the device, i.e. the serial number information provided via USB.
        /// </summary>
        public override string SerialNumber
        {
            get { return device.SerialNumber; }
        }

        /// <summary>
        /// List of reference amplitudes for the different channels at 10 MHz. These are stored in the firmware of the 
        /// USB controller when building the device.
        /// </summary>
        public override List<double> ReferenceAmplitude
        {
            get { return referenceAmplitude; }
        }

        #region Private Members

        private AD9958MessageFactory messageFactory;

        #endregion

        /// <summary>
        /// Function for debugging the way the reference amplitudes are extracted via EP1. Does not work
        /// yet and once it will work it will no longer be public.
        /// </summary>
        public void GetReferenceAmplitudes()
        {
            MasterReset();
            referenceAmplitude = new List<double>();
            sendToEP1(new Message(new byte[] { 0x04, 0x99, 0x64, 0x00 }));
            Message amplitudes = receiveFromEP1(6);
        }


        /// <summary>
        /// Private member storing the reference amplitudes of the different channels at 10MHz.
        /// </summary>
        private List<double> referenceAmplitude;

        #endregion

        /// Naming in the following region is consistent with the firmware naming of Christian
        /// These commands should not be called directly but are to be unit tested, hence they are internal.
        #region EP1OUT Commands

        /// <summary>
        /// Issue a Full_DDS_Reset.
        /// </summary>
        /// <remarks>
        /// This is an command defined on the firmware of the EZ USB FX2. It toggles the master reset bit of the DDS chip,
        /// initializes the DDS into the bytewise transfer mode and prepares the USB engine into a mode
        /// which enables EP2 transfers and switches on the IOUPDATE mode. However it does not set the CSR, CFR and FR1 to 
        /// the right configuration, so use MasterReset instead.
        /// </remarks>
        internal void Full_DDS_Reset()
        {
            log.Info("Sending Full_DDS_Reset command");
            Message fullResetMessage = new Message(new byte[] { 0x03, 0x08 });
            sendToEP1(fullResetMessage);
        }

        /// <summary>
        /// Enable transfer via EP2OUT
        /// </summary>
        /// <remarks>
        /// This is a command defined in the firmware of the EZ USB chip. It enables data transfer via EP2OUT (which triggers
        /// the GPIF engine).
        /// </remarks>
        internal void Start_Transfer()
        {
            if (log.IsInfoEnabled) { log.Info("Enabling transfer via EP2OUT"); }
            Message msg = new Message(new byte[] { 0x03, 0x03});
            sendToEP1(msg);
        }


        /// <summary>
        /// Disable transfer via EP2OUT
        /// </summary>
        /// <remarks> 
        /// This is a command defined in the firmware of the EZ USB chip. It disables data transfer via EP2OUT (which stops
        /// the GPIF routine).
        /// </remarks>
        internal void Stop_Transfer()
        {
            if (log.IsInfoEnabled) { log.Info("Disabling transfer via EP2OUT"); }
            Message msg = new Message(new byte[] { 0x03, 0x04});
            sendToEP1(msg);
        }

        /// <summary>
        /// Select ListplayMode with a given segment length
        /// </summary>
        /// <param name="SegmentLength">Length of each segment (must lie in [1,128])</param>
        /// <remarks>
        /// This is a command that is defined in the firmware of the EZ USB chip. It selects the mode where data, the list, is
        /// transferred into the RAM buffer in the USB device. After starting this mode a segment of the list of length SegmentLength
        /// is transferred into the DDS upon receiving a transition from logical low to high on P3, followed by an IOUpdate.
        /// Once the end of the list in the RAM is reached the buffer wraps around. To reset the list back to the first segment
        /// one can issue a logical high to P2.
        internal void ListplayMode(int SegmentLength)
        {
            if (log.IsInfoEnabled) { log.InfoFormat("Configuring listplay mode with segment length {0}", SegmentLength); }
            Message msg = new Message(0x04, 0x0b);
            msg.Add((byte)SegmentLength);
            sendToEP1(msg);
        }

        /// <summary>
        /// Start listplay mode.
        /// </summary>
        /// <remarks>
        /// This is a command that is defined in the firmware of the EZ USB chip. It starts the GPIF engine when
        /// listplay mode has been chosen.
        /// </remarks>
        internal void StartListplayMode()
        {
            if (log.IsInfoEnabled) { log.Info("Starting listplay mode"); }
            Message msg = new Message(0x03, 0x0c);
            sendToEP1(msg);
        }

        /// <summary>
        /// Stop listplay mode
        /// </summary>
        /// <remarks>
        /// This is a command that is defined in the firmware of the EZ USB chip. It stops the GPIF engine when
        /// listplay mode has been chosen.
        /// </remarks>
        internal void StopListplayMode()
        {
            if (log.IsInfoEnabled) { log.Info("Stopping listplay mode"); }
            Message msg = new Message(0x03, 0x0e);
            sendToEP1(msg);
        }

        /// <summary>
        /// Set both channels to 10 MHz and jump relative phase in steps of 45 deg. Only for debugging.
        /// May change and disappear at any instant.
        /// </summary>
        public void DebugListplayMode()
        {
            MasterReset();
            SetTwoChannelRelativePhase(10e6, 0);
            SelectChannelToWrite(0);
            Stop_Transfer();

            Message msg = new Message();
            msg.Add(messageFactory.SetPhaseMessage(0));
            int segmentLength = msg.Count;
            msg.Add(messageFactory.SetPhaseMessage(45));
            msg.Add(messageFactory.SetPhaseMessage(90));
            msg.Add(messageFactory.SetPhaseMessage(135));
            msg.Add(messageFactory.SetPhaseMessage(180));
            msg.Add(messageFactory.SetPhaseMessage(225));
            msg.Add(messageFactory.SetPhaseMessage(270));
            msg.Add(messageFactory.SetPhaseMessage(315));

            ListplayMode(segmentLength);
            StartListplayMode();
            sendToEP2(msg);
        }

        #endregion

        /// <summary>
        /// Do a master reset on the DDS, that is a FullDDSReset plus setting mode to singletone and
        /// levels to 2, which configures the right transfer mode on the DDS.
        /// </summary>
        public void MasterReset()
        {
            if (log.IsInfoEnabled) { log.Info("Performing a master reset."); }

            Full_DDS_Reset();

            log.Info("Initializing DDS as part of master reset.");
            
            Message initialization = new Message();
            initialization.Add(messageFactory.SelectChannelMessage(2));
            initialization.Add(messageFactory.SetModeMessage("singletone"));
            initialization.Add(messageFactory.SetLevelMessage(2));

            sendToEP2(initialization);
        }

        /// <summary>
        /// Select channel to write.
        /// </summary>
        /// <param name="ChannelNumber">Channel Number. 0,1 for single channel, 2 for both</param>
        public void SelectChannelToWrite(int ChannelNumber)
        {
            log.InfoFormat("Selecting channel {0} to write to", ChannelNumber);
            sendToEP2(messageFactory.SelectChannelMessage(ChannelNumber));
        }

        /// <summary>
        /// Select modulation mode.
        /// Currently only singletone operation is supported, meaning no modulation.
        /// </summary>
        /// <param name="Mode">{"singletone"}</param>
        public void SetMode(string Mode)
        {
            log.InfoFormat("Selecting mode {0}", Mode);
            Message msg = new Message();
            msg.Add(messageFactory.SelectChannelMessage(2));
            msg.Add(messageFactory.SetModeMessage(Mode));
            sendToEP2(msg);
        }

        /// <summary>
        /// Set modulation levels.
        /// Currently only two levels are supported.
        /// </summary>
        /// <param name="Levels">{2}</param>
        public void SetLevels(int Levels)
        {
            log.InfoFormat("Setting levels to {0}", Levels);
            sendToEP2(messageFactory.SetLevelMessage(Levels));
        }


        /// <summary>
        /// Set frequency of specified channel.
        /// </summary>
        /// <param name="Channel">Channel Number. 0/1 for single channel, 2 for both</param>
        /// <param name="Frequency">Frequency in Hz</param>
        public void SetFrequency(int ChannelNumber, double Frequency)
        {
            log.InfoFormat("Setting frequency of channel {0} to {1:0.000e0} Hz", ChannelNumber, Frequency);
            Message msg = new Message();
            msg.Add(messageFactory.SelectChannelMessage(ChannelNumber));
            msg.Add(messageFactory.SetFrequencyMessage(Frequency));
            sendToEP2(msg);
        }

        /// <summary>
        /// Set frequency of currently active channel.
        /// </summary>
        /// <param name="Frequency">Frequency in Hz</param>
        /// <remarks>
        /// Which channel's frequency is set is determined by the current setting in 
        /// in the Channel Select Register on the DDS. This can be changed by calling
        /// the SelectChannelToWrite method.
        /// </remarks>
        public void SetFrequency(double Frequency)
        {
            log.InfoFormat("Setting frequency of current channel to {0:0.000e0}", Frequency);
            sendToEP2(messageFactory.SetFrequencyMessage(Frequency));
        }

        /// <summary>
        /// Set amplitude in units of the scale factor of the DDS. CAUTION:
        /// using this function requires a little understanding of the DDS.
        /// </summary>
        /// <param name="AmplitudeScaleFactor">Amplitude Scale Factor, consult data sheet</param>
        public void SetAmplitude(int AmplitudeScaleFactor)
        {
            log.InfoFormat("Setting amplitude scale factor for current channel to {0}", AmplitudeScaleFactor);
            sendToEP2(messageFactory.SetAmplitudeMessage(AmplitudeScaleFactor));
        }


        /// <summary>
        /// Set both channels to the same frequency with a relative phase.
        /// </summary>
        /// <note>
        /// This function assumes that the two channels have an intrinsic phase shift of pi (wrong polarity of transformer on board).
        /// It adds a phase offset to the second channel (1=sin(Frequency*t), 2=sin(Frequency*t+Phase).
        /// </note>
        /// <param name="Frequency">Frequency in Hertz</param>
        /// <param name="RelativePhase">Phase in degree</param>
        public void SetTwoChannelRelativePhase(double Frequency, double RelativePhase)
        {
            log.InfoFormat("Setting both channels to {0:0.000e0} with a relative phase of {1}", Frequency, RelativePhase);

            MasterReset();

            #warning generateSetPhaseMessage assumes that there is an intrinsic phase shift of pi on the board
            double phaseShiftOnBoard = 180;

            if (log.IsInfoEnabled) { log.InfoFormat("I am assuming a phase shift on the board of {0} (sent = set - {0})", phaseShiftOnBoard); }

            Message msg = new Message();
            msg.Add(messageFactory.SetLevelMessage(2));
            msg.Add(messageFactory.SelectChannelMessage(2));
            msg.Add(messageFactory.SetModeMessage("singletone"));
            msg.Add(messageFactory.SelectChannelMessage(0));
            msg.Add(messageFactory.SetFrequencyMessage(Frequency));
            msg.Add(messageFactory.SetPhaseMessage(0));
            msg.Add(messageFactory.SelectChannelMessage(1));
            msg.Add(messageFactory.SetFrequencyMessage(Frequency));
            msg.Add(messageFactory.SetPhaseMessage(RelativePhase - phaseShiftOnBoard));

            sendToEP2(msg);
        }

        /// <summary>
        /// Set DDS to modulation mode via the 4 bit register.
        /// </summary>
        /// <param name="Channel">Channel which should be modulated</param>
        /// <param name="Levels">Number of modulation levels</param>
        /// <param name="ModulationType">Modulation type, e.g. fm, am, pm</param>
        /// <param name="ChannelWordList">List of channel words, e.g. frequencies or phases.</param>
        public void SetModulation(int Channel, int Levels, string ModulationType, params double[] ChannelWordList)
        {
            if (log.IsInfoEnabled)
            {
                log.InfoFormat("Setting {0} level modulation mode \"{1}\" for channel {2}",Levels, ModulationType, Channel);
            }
            StringBuilder channelWords = new StringBuilder();
            channelWords.Append("The channel words are: ");
            foreach (double word in ChannelWordList)
            {
                channelWords.AppendFormat("{0:0.000e0}, ", word);
            }
            if (log.IsInfoEnabled) { log.Info(channelWords.ToString()); }

            sendToEP2(messageFactory.SetModulationMessage(Channel, Levels, ModulationType, ChannelWordList));
        }

        public void SetFrequencyList(params double[] Frequency)
        {
            Stop_Transfer();

            Message msg = new Message();
            msg.Add(messageFactory.SetFrequencyMessage(Frequency[0]));
            int segmentLength = msg.Count;

            for (int k = 1; k < Frequency.Length; k++)
            {
                msg.Add(messageFactory.SetFrequencyMessage(Frequency[k]));
            }

            ListplayMode(segmentLength);
            StartListplayMode();
            sendToEP2(msg);
        }
        
        #region Functions for sending and receiving messages

        /// <summary>
        /// Send message to EP1. This routine will append a checksum byte at the end of the message. So 
        /// don't include it in the message passed to this routine
        /// </summary>
        /// <param name="message">Message to be sent without checksum byte</param>
        private void sendToEP1(Message message)
        {
            if (log.IsDebugEnabled) { log.DebugFormat("Recieved message to send to EP1{0}", message.ToString()); }

            byte checksum = DDSUtils.Checksum(message);

            if (log.IsDebugEnabled) { log.DebugFormat("Appending checksum byte {0} to message", checksum); }

            message.Add(checksum);
            
            if (log.IsDebugEnabled) { log.DebugFormat("Sending message to EP1: {0}", message.ToString()); }
            device.SendDataToEP1(message.ToArray());
        }

        private void sendToEP2(Message message)
        {
            log.DebugFormat("Sending message to EP2: {0}",message.ToString());
            device.SendDataToEP2(message.ToArray());
        }

        private Message receiveFromEP1(int Length)
        {
            byte[] bytes = new byte[Length];
            device.ReceiveDataFromEP1(ref bytes);
            return new Message(bytes);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a AD9958 from an IDDS Microcontroller device.
        /// </summary>
        /// <param name="usbDevice">Microcontroller device</param>
        /// <remarks>
        /// This constructor is the one that is to be used when programming in .NET since it neatly splits
        /// the CyUSB functionality from the CQTDDS functionality (the IDDS Microcontroller abstracts from 
        /// the CyUSB functionality and makes the whole thing unit testable. However Labview crashes if 
        /// one tries to use it in that way, so use the other constructors which hardwire functionality
        /// of CyUSB
        /// </remarks>
        public AD9958(IDDSUSBChip usbDevice)
        {
            // device is protected member of the base class
            device = usbDevice;
            initializeAD9958();
        }

        /// <summary>
        /// Create a ADD9958 by looking up a specific connected device by number. This is not a particularly 
        /// reliable method since the order of the devices depends on the order in which they have been plugged in
        /// </summary>
        /// <param name="DeviceNumber">Device number.</param>
        public AD9958(int DeviceNumber)
        {
            USBDeviceList deviceList = new USBDeviceList(CyConst.DEVICES_CYUSB);
            device = (new DDSUSBChip(deviceList[DeviceNumber] as CyUSBDevice));
            initializeAD9958();
        }

        private void initializeAD9958()
        {
            messageFactory = new AD9958MessageFactory();
        }
        #endregion



    }
}
