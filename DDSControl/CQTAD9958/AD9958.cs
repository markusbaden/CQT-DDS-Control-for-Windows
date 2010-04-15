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

        /// <summary>
        /// List of supported moudlation modes.
        /// </summary>
        public List<string> SupportedModulationModes
        {
            get { return ampFreqPhasePatternCFR.Keys.ToList(); }
        }

        /// <summary>
        /// List of supported modulation levels.
        /// </summary>
        public List<int> SupportedModulationLevels
        {
            get { return levelPatternFR1.Keys.ToList(); }
        }

        /// <summary>
        /// Byte pattern in the channel select register that corresponds to the transfer mode
        /// (i.e. with no channel selected). This is combined with the bits corresponding to 
        /// which channel are selected to generate the call the channel select register.
        /// Intended for debugging. Consult data sheet for what it means.
        /// </summary>
        public byte WritePatternCSR
        {
            get { return writePatternCSR; }
        }

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
            msg.Add(generateSetPhaseMessage(0));
            int segmentLength = msg.Count;
            msg.Add(generateSetPhaseMessage(45));
            msg.Add(generateSetPhaseMessage(90));
            msg.Add(generateSetPhaseMessage(135));
            msg.Add(generateSetPhaseMessage(180));
            msg.Add(generateSetPhaseMessage(225));
            msg.Add(generateSetPhaseMessage(270));
            msg.Add(generateSetPhaseMessage(315));

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
            initialization.Add(generateSelectChannelMessage(2));
            initialization.Add(generateSetModeMessage("singletone"));
            initialization.Add(generateSetLevelMessage(2));


            sendToEP2(initialization);
        }

        /// <summary>
        /// Select channel to write.
        /// </summary>
        /// <param name="ChannelNumber">Channel Number. 0,1 for single channel, 2 for both</param>
        public void SelectChannelToWrite(int ChannelNumber)
        {
            log.InfoFormat("Selecting channel {0} to write to", ChannelNumber);
            sendToEP2(generateSelectChannelMessage(ChannelNumber));
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
            msg.Add(generateSelectChannelMessage(2));
            msg.Add(generateSetModeMessage(Mode));
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
            sendToEP2(generateSetLevelMessage(Levels));
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
            msg.Add(generateSelectChannelMessage(ChannelNumber));
            msg.Add(generateSetFrequencyMessage(Frequency));
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
            sendToEP2(generateSetFrequencyMessage(Frequency));
        }

        /// <summary>
        /// Set amplitude in units of the scale factor of the DDS. CAUTION:
        /// using this function requires a little understanding of the DDS.
        /// </summary>
        /// <param name="AmplitudeScaleFactor">Amplitude Scale Factor, consult data sheet</param>
        public void SetAmplitude(int AmplitudeScaleFactor)
        {
            log.InfoFormat("Setting amplitude scale factor for current channel to {0}", AmplitudeScaleFactor);
            sendToEP2(generateSetAmplitudeMessage(AmplitudeScaleFactor));
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
            msg.Add(generateSetLevelMessage(2));
            msg.Add(generateSelectChannelMessage(2));
            msg.Add(generateSetModeMessage("singletone"));
            msg.Add(generateSelectChannelMessage(0));
            msg.Add(generateSetFrequencyMessage(Frequency));
            msg.Add(generateSetPhaseMessage(0));
            msg.Add(generateSelectChannelMessage(1));
            msg.Add(generateSetFrequencyMessage(Frequency));
            msg.Add(generateSetPhaseMessage(RelativePhase-phaseShiftOnBoard));

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
            
            Message msg = new Message();
            msg.Add(generateSelectChannelMessage(Channel));
            msg.Add(generateSetLevelMessage(Levels));
            msg.Add(generateSetModeMessage(ModulationType));

            switch (ModulationType)
            {
                case "fm":
                    msg.Add(generateSetFrequencyMessage(ChannelWordList[0]));
                    byte[] channelRegisterWord = calculateFrequencyTuningWordAsBytes(ChannelWordList[1]);
                    msg.Add(generateSetChannelWordMessage(1, channelRegisterWord));
                    break;
                case "pm":
                    msg.Add(generateSetPhaseMessage(ChannelWordList[0]));
                    
                    // Fill the Channel Word Registers
                    // They are 4byte long and the PhaseOffsetWord only 14bit long
                    // the PhaseOffsetWord has to be MSB aligned in the register
                    int PhaseOffsetWord = calculatePhaseOffsetWord(ChannelWordList[1]);
                    
                    // First shift the word by two bits to the left to make it MSB aligned
                    // for a two byte register
                    PhaseOffsetWord = PhaseOffsetWord << 2;
                    
                    // Then convert it to MSBytearray
                    Message phaseByte = DDSUtils.IntToMSByteArray(PhaseOffsetWord,2);
                    
                    // Finally make sure that the two last bits are zero by doing a
                    // bitwise and with 1111 1100 = 0xFC
                    phaseByte[1] = (byte) (phaseByte[1] & 0xFC);

                    // Add to zero bytes for the remaining byte registers
                    phaseByte.AddRange(new byte[] {0x00, 0x00});
                    msg.Add(generateSetChannelWordMessage(1,phaseByte.ToArray()));
                    
                    break;
                default:
                    if (log.IsErrorEnabled) { log.ErrorFormat("Could not recognize the modulatioin type {0}", ModulationType); }
                    break;
            }


            sendToEP2(msg);
        }
        
        #region Functions to generate messages

        private Message generateSelectChannelMessage(int ChannelNumber)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["CSR"].Address);
            msg.Add(new byte[] { (byte)(channelPatternCSR[ChannelNumber] + writePatternCSR) });
            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to select channel {0}: {1}", ChannelNumber, msg); }
            return msg;
        }

        private Message generateSetChannelWordMessage(int ChannelRegisterNumber, byte[] Content)
        {
            Message msg = new Message();
            byte addressByte = (byte)(0x0A + (ChannelRegisterNumber - 1));
            msg.Add(addressByte);
            msg.Add(Content);

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set Channel Register {0} to given content: {1}", ChannelRegisterNumber,msg); }

            return msg;
        }

        private Message generateSetAmplitudeMessage(int AmplitudeScaleFactor)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["ACR"].Address);
            // Amplitude is set manually so amplitude ramp rate all zeros
            byte acr23to16 = 0x00;

            // ACR[9:8] are the most significant bits of the amplitude
            // scale factor (obtained by shift magic) plus set 
            // ACR[12] to 1 to enable manual amplitude setting
            byte acr15to8 = (byte)((AmplitudeScaleFactor >> 8) + 0x10);
            byte acr7to0 = (byte)(AmplitudeScaleFactor & 0xFF);
            msg.Add(new byte[] { acr23to16, acr15to8, acr7to0 });

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set current amplitude to scalefac {0}: {1}", AmplitudeScaleFactor,msg); }
            return msg;
        }

        private Message generateSetFrequencyMessage(double Frequency)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["CFTW"].Address);
            msg.Add(calculateFrequencyTuningWordAsBytes(Frequency));
            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set current frequency to {0:0.000e0}: {1}", Frequency, msg); }
            return msg;
        }

        private Message generateSetLevelMessage(int Levels)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["FR1"].Address);
            
            byte levelByte = levelPatternFR1[Levels];

            // 0xa8 and 0x20 take from Christians implementation
            msg.Add(new byte[] { 0xa8, levelByte, 0x20 });

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set levels to {0}: {1}", Levels, msg); }

            return msg;
        }
        
        private Message generateSetModeMessage(string Mode)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["CFR"].Address);
            
            // CFR part
            byte ampFreqPhaseByte = ampFreqPhasePatternCFR[Mode];

            // 0x03 and 0x00 taken from Christian's implementation
            msg.Add(new byte[] { ampFreqPhaseByte, 0x03, 0x00 });

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set mode to {0}: {1}", Mode, msg); }
            
            return msg;
        }

        private Message generateSetPhaseMessage(double Phase)
        {

            double moduloPhase = calculateModuloPhase(Phase);

            if (log.IsDebugEnabled) { log.DebugFormat("Setting phase modulo 360, i.e. {0} -> {1} ", Phase, moduloPhase); }
            
            Message msg = new Message();
            msg.Add(registerByShortName["CPOW"].Address);
            
            int PhaseOffsetWord = calculatePhaseOffsetWord(moduloPhase);

            // The CPOW register is two bytes long (16 bit). The PhaseOffsetWord is 
            // LSB aligned, i.e. the two most significant bits of byte1 are don't care,
            // the rest of byte1 are bits 13:8 of the word and byte2 are bits 7:0 of the
            // word.
            // We first right shift the word by 8 bit to store the bits 13:8 (i.e. 15:8 -> 7:0)
            // and then set the two highest bits (formerly 15:14, now 7:6) to zero by doing a bitwise 
            // and with 0011 1111 = 0x3F. For a correct word these should be zero anyway.
            byte byte1 = (byte)( (PhaseOffsetWord >> 8) &0x3f);

            // In order to store bits 7:0 we first blank out the higher bits by doing a bitwise
            // and with 1111 1111 = 0xFF and then store it
            byte byte2 = (byte)( PhaseOffsetWord&0xff );

            msg.Add(new byte[] { byte1, byte2 });

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set current phase to {0}: {1}", moduloPhase, msg); }

            return msg;
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

        #region Functions for calculating words
        
        private int calculateFrequencyTuningWord(double frequency)
        {
            return (int)(frequency * frequencyStep);
        }

        private byte[] calculateFrequencyTuningWordAsBytes(double frequency)
        {
            int FTW = calculateFrequencyTuningWord(frequency);
            return DDSUtils.IntToMSByteArray(FTW).ToArray();
        }

        private int calculatePhaseOffsetWord(double phase)
        {
            return (int)(phase * phaseStep);
        }

        private double calculateModuloPhase(double Phase)
        {
            double moduloPhase = Phase % 360;
            if (moduloPhase < 0)
                moduloPhase = moduloPhase + 360;
            return moduloPhase;
        }

        #endregion

        private double clockFrequency = 500e6;
        private double frequencyStep;
        private double phaseStep;
        


        #region Byte patterns

        /// <summary>
        /// Dictionary thar stores the byte that corresponds to select channel 0,1 or both
        /// in the Channel Select Register (CSR) of the AD9958. This bit pattern
        /// can be combined by bitwise operations to the generate valid CSR bytes.
        /// </summary>
        /// <remarks>
        /// Select Channel 0 = 01000000 = 0x40
        /// Select Channel 1 = 10000000 = 0x80
        /// Select both      = 11000000 = 0xc0
        /// </remarks>
        private Dictionary<int, byte> channelPatternCSR;
        
        /// <summary>
        /// Byte that corresponds to correct write mode in the ChannelSelectRegister (CSR) of
        /// the AD9958. This bit pattern can be combined by bitwise operations to generate valid CSR 
        /// bytes.
        /// </summary>
        /// <remarks>
        ///  00110000 = Is open,
        /// +00000110 = LSB first, 4bit Serial mode
        /// ----------------------------------------
        ///  01100110 = 0x36
        /// </remarks>
        private byte writePatternCSR = 0x36;


        /// <summary>
        /// Dictionary thar stores the byte that corresponds to select different modulation levels
        /// in the Function Register 1 of the AD9958. ATM only two levels no ramp is supported
        /// </summary>
        /// <remarks>
        /// 00000000 = 0x00 = two level no ramp = 2
        /// </remarks>
        private Dictionary<int, byte> levelPatternFR1;

        /// <summary>
        /// Dictionary thar stores the byte that corresponds to select single tone, amplitude modulation,
        /// frequency modulation or phase modulation in the Channel Function Register (CFR). ATM only
        /// single tone operation is supported.
        /// </summary>
        /// <remarks>
        /// 00000000 = 0x00 = no modulation (single_tone)
        /// 01000000 = 0x40 = amplitude_modulation
        /// 10000000 = 0x80 = frequency_modulation
        /// 11000000 = 0xc0 = phase_modulation
        /// </remarks>
        private Dictionary<string, byte> ampFreqPhasePatternCFR;

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
        /// reliable method since the order of the devices depends on the order in which they have been pugged in
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
            initializeRegisters();

            defineChannelPattern();
            defineAmpFreqPhasePattern();
            defineLevelPattern();
            defineFrequencyConstants();
            definePhaseConstants();
        }


        #endregion

        #region Initialization routines

        private void initializeRegisters()
        {
            // registerByShortName is a protectedMember in the base class
            registerByShortName = new Dictionary<string, DDSRegister>();
            
            // For the definition of the following registers refer to the
            // AD9958 data sheet.
            // Channel Select Register (CSR), use data sheet default value as default
            addRegister("CSR", 0x00, new byte[] { 0xF0 });
            
            // Function Register 1 (FR1), the default values that are different then those
            // in the data sheet are extracted from Christian' implementation
            // DefaultValues[0] = FR1[23:16] = 0xa8, i.e. PLL multiplication factor = 10 and VCO Gain high
            // This sets the multiplication factor from the ref. oscillation to the onboard clock. The CQT DDS
            // runs of a 50Mhz VCO at this moment, so in order to get the 500Mhz clock we have to hava a PLL 
            // multiplication factor of 10.
            // DefaultValues[1] = FR1[15:8] = 0x00, as in the data sheet
            // DefaultValues[2] = FR1[7:0] = 0x20, i.e. sync clock pin is disabled.
            addRegister("FR1", 0x01, new byte[] { 0xa8, 0x00, 0x20 });

            // Channel Function Register (CFR), the default values that are different then those
            // in the data sheet are extracted from Christian' implementation
            // DefaultValues[0] = CFR[23:16] = 0x00, as in the data sheet
            // DefaultValues[1] = CFR[15:8] = 0x03, as in the data sheet
            // DefaultValues[2] = CFR[7:0] = 0x00, clear phase accumulator bit (CFR[1]) is set to zero.
            addRegister("CFR", 0x03,new byte[] {0x00, 0x03, 0x00});

            // Channel Frequency Tuning Word (CFTW) Register 
            addRegister("CFTW", 0x04, new byte[] { 0x00, 0x00, 0x00, 0x00 });
            
            // Channel Phase Offset Word (CPOW) Register
            addRegister("CPOW", 0x05, new byte[] { 0x00, 0x00 });
            
            // Amplitude Control Regist (ACR)
            addRegister("ACR", 0x06, new byte[] { 0x00, 0x00, 0x00 });
        }

        private void addRegister(string Name, byte Address, byte[] DefaultValues)
        {
            registerByShortName.Add(Name, new DDSRegister(Name, Address, DefaultValues));
        }

        private void defineChannelPattern()
        {
            channelPatternCSR = new Dictionary<int,byte>();
            channelPatternCSR.Add(0, 0x40);
            channelPatternCSR.Add(1, 0x80);
            channelPatternCSR.Add(2, 0xc0);
        }

        private void defineAmpFreqPhasePattern()
        {
            ampFreqPhasePatternCFR = new Dictionary<string, byte>();
            // If you add am, fm and pm support make sure you change 
            // the comment at the dict definition.
            ampFreqPhasePatternCFR.Add("singletone", 0x00);
            ampFreqPhasePatternCFR.Add("fm", 0x80);
            ampFreqPhasePatternCFR.Add("pm", 0xC0);
        }

        private void defineLevelPattern()
        {
            levelPatternFR1 = new Dictionary<int, byte>();
            levelPatternFR1.Add(2, 0x00);
        }
        
        private void defineFrequencyConstants()
        {
            // Fout = (FrequencyTuningWord * clockFrequency) / (2^32)
            // <=> FTW = Fout * frequencyStep
            // And use long for the calculation because int is int32 < 2^31
            frequencyStep = (((long)1 << 32) / clockFrequency);
        }

        private void definePhaseConstants()
        {
            // PhaseOffset = (PhaseOffsetWord/(2^14)) * 360
            // <=> POW = PhaseOffset * phaseStep
            phaseStep = Math.Pow(2, 14) / 360;
        }

        #endregion

    }
}
