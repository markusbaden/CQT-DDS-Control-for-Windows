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
        #endregion

        #region Private Members

        private AD9958MessageFactory messageFactory;

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
        /// </remarks>
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

        #endregion

        /// <summary>
        /// Do a master reset on the DDS, that is a FullDDSReset plus setting mode to singletone and
        /// levels to 2, which configures the right transfer mode on the DDS.
        /// </summary>
        public void MasterReset()
        {
            if (log.IsInfoEnabled) { log.Info("Performing a master reset."); }

            Full_DDS_Reset();

            // Since board revision too the DDS needs some time to process the Full_DDS_Reset
            // So we will sleep for ten milliseconds
            System.Threading.Thread.Sleep(10);

            log.Info("Initializing DDS as part of master reset.");
            
            Message initialization = new Message();
            initialization.Add(messageFactory.SelectChannel(2));
            initialization.Add(messageFactory.SetMode("singletone"));
            initialization.Add(messageFactory.SetLevel(2));

            sendToEP2(initialization);
        }

        /// <summary>
        /// Select channel to write.
        /// </summary>
        /// <param name="ChannelNumber">Channel Number. 0,1 for single channel, 2 for both</param>
        public void SelectChannelToWrite(int ChannelNumber)
        {
            log.InfoFormat("Selecting channel {0} to write to", ChannelNumber);
            sendToEP2(messageFactory.SelectChannel(ChannelNumber));
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
            msg.Add(messageFactory.SelectChannel(2));
            msg.Add(messageFactory.SetMode(Mode));
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
            sendToEP2(messageFactory.SetLevel(Levels));
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
            msg.Add(messageFactory.SelectChannel(ChannelNumber));
            msg.Add(messageFactory.SetFrequency(Frequency));
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
            sendToEP2(messageFactory.SetFrequency(Frequency));
        }

        /// <summary>
        /// Set amplitude in units of the scale factor of the DDS. CAUTION:
        /// using this function requires a little understanding of the DDS.
        /// </summary>
        /// <param name="AmplitudeScaleFactor">Amplitude Scale Factor, consult data sheet</param>
        public void SetAmplitude(int AmplitudeScaleFactor)
        {
            log.InfoFormat("Setting amplitude scale factor for current channel to {0}", AmplitudeScaleFactor);
            sendToEP2(messageFactory.SetAmplitude(AmplitudeScaleFactor));
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
            msg.Add(messageFactory.SetLevel(2));
            msg.Add(messageFactory.SelectChannel(2));
            msg.Add(messageFactory.SetMode("singletone"));
            msg.Add(messageFactory.SelectChannel(0));
            msg.Add(messageFactory.SetFrequency(Frequency));
            msg.Add(messageFactory.SetPhase(0));
            msg.Add(messageFactory.SelectChannel(1));
            msg.Add(messageFactory.SetFrequency(Frequency));
            msg.Add(messageFactory.SetPhase(RelativePhase - phaseShiftOnBoard));

            sendToEP2(msg);
        }

        #region Modulation Mode Commands

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
                log.InfoFormat("Setting {0} level modulation mode \"{1}\" for channel {2}", Levels, ModulationType, Channel);
            }
            StringBuilder channelWords = new StringBuilder();
            channelWords.Append("The channel words are: ");
            foreach (double word in ChannelWordList)
            {
                channelWords.AppendFormat("{0:0.000e0}, ", word);
            }
            if (log.IsInfoEnabled) { log.Info(channelWords.ToString()); }

            Message msg = new Message();
            msg.Add(messageFactory.SelectChannel(Channel));
            msg.Add(messageFactory.SetLevel(Levels));
            msg.Add(messageFactory.SetMode(ModulationType));
            msg.Add(messageFactory.FillChannelWords(ModulationType, ChannelWordList));
            sendToEP2(msg);
        }
 
        #endregion

        #region Linear Sweep Mode Commands

        /// <summary>
        /// Set DDS to perform linear frequency sweep. The registers for the rising and falling sweep will
        /// be connfigured with the same parameters.
        /// </summary>
        /// <param name="Channel">Which channel, 0/1 or 2 for both</param>
        /// <param name="StartFrequency">Start frequency</param>
        /// <param name="StopFrequency">Stop frequency</param>
        /// <param name="DeltaTime">Time between increments resp. decrements. Corresponds to ramp rate in the data sheet.</param>
        /// <param name="DeltaFrequency">Frequency increment resp. decrement. Corresponds to delta word in the data sheet.</param>
        public void SetLinearSweep(int Channel, double StartFrequency, double StopFrequency, double DeltaTime, double DeltaFrequency)
        {
            Message msg = new Message();
            msg.Add(messageFactory.SelectChannel(Channel));
            msg.Add(messageFactory.SetLevel(2));
            msg.Add(messageFactory.SetMode("fm", true, false));
            msg.Add(messageFactory.SetFrequency(StartFrequency));
            msg.Add(messageFactory.SetChannelWord(1, messageFactory.FrequencyMessage(StopFrequency).ToArray()));
            msg.Add(messageFactory.SetRampRate(DeltaTime));
            msg.Add(messageFactory.SetDeltaFrequency(DeltaFrequency));
            sendToEP2(msg);
        }

        /// <summary>
        /// Set DDS to perform differential sweep. Channel 0 will perform a linear sweep at
        /// the minimum sweep rate of Smin = 50kHz/s and channel 1 a sweep at Smin plus dS.
        /// The result is a sweep of the difference in the to channels with rate dS.
        /// </summary>
        /// <param name="DifferentialSlope">Differential slope dS of the sweep of the difference of the two channels</param>
        public void SetDifferentialSweep(double DifferentialSlope)
        {
            double deltaTime = deltaTimeForDifferentialSweep(DifferentialSlope);

            Message msg = new Message();
            msg.Add(messageFactory.SelectChannel(0));
            msg.Add(messageFactory.SetRampRate(maxDeltaTime));
            msg.Add(messageFactory.SetDeltaFrequency(minDeltaFrequency));
            msg.Add(messageFactory.SelectChannel(1));
            msg.Add(messageFactory.SetRampRate(deltaTime));
            msg.Add(messageFactory.SetDeltaFrequency(minDeltaFrequency));

            sendToEP2(msg);
        }

        private double maxDeltaTime = 2.048e-6;
        private double minDeltaFrequency = 0.1;

        private double deltaTimeForDifferentialSweep(double DifferentialSlope)
        {
            // Slope of channel 0 is minimum slope f_min/t_max
            double minSlope = minDeltaFrequency / maxDeltaTime;

            // Slope of channel 1 is S1=S0 + S
            // fixing f2=fmin we get (fixing t2=tmax is bad since it leads to S1_min = fmin/tmax)
            // t2 = t_max / (1+S/Smin) = t_max / (1+S*tmax/fmin)
            double deltaTime = maxDeltaTime / (1 + DifferentialSlope * maxDeltaTime / minDeltaFrequency);
            return deltaTime;        
        }

        #endregion

        #region Listplay Mode Commands

        public void SetFrequencyList(params double[] Frequency)
        {

            Message msg = new Message();
            msg.Add(messageFactory.SetFrequency(Frequency[0]));
            int segmentLength = msg.Count;

            for (int k = 1; k < Frequency.Length; k++)
            {
                msg.Add(messageFactory.SetFrequency(Frequency[k]));
            }

            Stop_Transfer();
            ListplayMode(segmentLength);
            StartListplayMode();
            sendToEP2(msg);
        }

        public void SetFrequencyAmplitudeList(double[] Frequency, double[] RelativeAmplitude)
        {
            // Convert Relative Amplitude to amplitude scalefactor
            // The max amplitude scalefactor is hard coded in the for loop
            List<int> ampScaleFactors = new List<int>();
            
            for (int k = 0; k < RelativeAmplitude.Length; k++)
            {
                ampScaleFactors.Add((int) Math.Round(RelativeAmplitude[k] * 1023));
            }
            
            Message msg = new Message();
            
            // Add first frequency amplitude to get segment list
            msg.Add(messageFactory.SetFrequency(Frequency[0]));
            msg.Add(messageFactory.SetAmplitude(ampScaleFactors[0]));
            
            int segmentLength = msg.Count;
            for (int k = 1; k < Frequency.Length; k++)
            {
                msg.Add(messageFactory.SetFrequency(Frequency[k]));
                msg.Add(messageFactory.SetAmplitude(ampScaleFactors[k]));
            }
        
            startListplayModeWithMessage(msg, segmentLength);
        }

        public void SetDeltaFrequencyList(params double[] DeltaFrequency)
        {
            Message msg = new Message();
            msg.Add(messageFactory.SetDeltaFrequency(DeltaFrequency[0]));

            int segmentLength = msg.Count;

            for (int k = 1; k < DeltaFrequency.Length; k++)
            {
                msg.Add(messageFactory.SetDeltaFrequency(DeltaFrequency[k]));
            }

            startListplayModeWithMessage(msg, segmentLength);
        }


        public void SetDifferentialSweepList(params double[] Slopes)
        {
            Message msg = new Message();
            double currentDelta = deltaTimeForDifferentialSweep(Slopes[0]);
            msg.Add(messageFactory.SetRampRate(currentDelta));
            int segmentLength = msg.Count;

            for (int k = 1; k < Slopes.Length; k++)
            {
                currentDelta = deltaTimeForDifferentialSweep(Slopes[k]);
                msg.Add(messageFactory.SetRampRate(currentDelta));
            }
            startListplayModeWithMessage(msg, segmentLength);
        
        }

        private void startListplayModeWithMessage(Message message, int SegmentLength)
        {
            Stop_Transfer();
            ListplayMode(SegmentLength);
            StartListplayMode();
            sendToEP2(message);
        }
        #endregion

        #region Functions for sending and receiving messages

        /// <summary>
        /// Send message to EP1. This routine will append a checksum byte at the end of the message. So 
        /// don't include it in the message passed to this routine
        /// </summary>
        /// <param name="message">Message to be sent without checksum byte</param>
        private void sendToEP1(Message message)
        {
            if (log.IsDebugEnabled) { log.DebugFormat("Recieved message to send to EP1: {0}", message.ToString()); }

            byte checksum = DDSUtils.Checksum(message);

            if (log.IsDebugEnabled) { log.DebugFormat("Appending checksum byte 0x{0} to message", checksum); }

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

        /// <summary>
        /// Create a ADD9958 by looking up a specific connected device by serial number.
        /// </summary>
        /// <param name="DeviceNumber">Device number.</param>
        public AD9958(string SerialNumber)
        {
            // A list of the serial numbers of all currently connected
            // boards is already implemented in CyLabviewHelper, so we are going to use that one
            CyLabviewHelper helper = new CyLabviewHelper();
            List<string> serialNumbers = helper.ConnectedDDSBoards();

            // Find device number by serial
            int deviceNumber = 0;
            bool found = false;
            for (int i = 0; i < serialNumbers.Count; i++)
            {
                if (String.Equals(SerialNumber, serialNumbers[i], StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                    deviceNumber = i;
                    break;
                }
            }

            // Check whether the serial number was found, if not then throw exception
            // Otherwise the device with deviceNumber 0 would be returned.
            if (!found)
            {
                throw new System.ArgumentException("Invalid serial number, no such device connected", "SerialNumber");
            }

            USBDeviceList deviceList = new USBDeviceList(CyConst.DEVICES_CYUSB);
            device = (new DDSUSBChip(deviceList[deviceNumber] as CyUSBDevice));
            initializeAD9958();
        }

        private void initializeAD9958()
        {
            messageFactory = new AD9958MessageFactory();
        }
        #endregion
    }
}
