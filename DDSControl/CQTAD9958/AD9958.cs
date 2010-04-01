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

        public override string Description
        {
            get { return device.Product; }
        }
        
        public override string SerialNumber
        {
            get { return device.SerialNumber; }
        }

        public List<string> SupportedModulationModes
        {
            get { return ampFreqPhasePatternCFR.Keys.ToList(); }
        }

        public List<int> SupportedModulationLevels
        {
            get { return levelPatternFR1.Keys.ToList(); }
        }

        public byte WritePatternCSR
        {
            get { return writePatternCSR; }
        }

        /// <summary>
        /// Send full reset command via Endpoint 1.
        /// </summary>
        public void FullReset()
        {
            log.Info("Performing full reset");
            byte[] fullResetByte = new byte[] {0x03,0x08,0x0b};
            device.SendDataToEP1(fullResetByte);
        }

        /// <summary>
        /// Select channel to write.
        /// </summary>
        /// <param name="ChannelNumber">Channel Number. 0,1 for single channel, 2 for both</param>
        public void SelectChannelToWrite(int ChannelNumber)
        {
            sendToEP2(generateSelectChannelMessage(ChannelNumber));
        }

        /// <summary>
        /// Select modulation mode.
        /// Currently only singletone operation is supported, meaning no modulation.
        /// </summary>
        /// <param name="Mode">{"singletone"}</param>
        public void SetMode(string Mode)
        {
            #warning SetMode is still sequential (not single message but two)
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
            sendToEP2(generateSetLevelMessage(Levels));
        }


        /// <summary>
        /// Set frequency of specified channel.
        /// </summary>
        /// <param name="Channel">Channel Number. 0/1 for single channel, 2 for both</param>
        /// <param name="Frequency">Frequency in Hz</param>
        public void SetFrequency(int ChannelNumber, double Frequency)
        {
            #warning SetFrequency of channel is still sequential (not single message but two)
            SelectChannelToWrite(ChannelNumber);
            SetFrequency(Frequency);
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
            sendToEP2(generateSetFrequencyMessage(Frequency));
        }

        /// <summary>
        /// Set amplitude in units of the scale factor of the DDS. CAUTION:
        /// using this function requires a little understanding of the DDS.
        /// </summary>
        /// <param name="AmplitudeScaleFactor">Amplitude Scale Factor, consult data sheet</param>
        public void SetAmplitude(int AmplitudeScaleFactor)
        {
            sendToEP2(generateSetAmplitudeMessage(AmplitudeScaleFactor));
        }

        public void SetTwoChannelRelativePhase(double Frequency, double RelativePhase)
        {
            log.InfoFormat("Setting both channels to {0:0.000e0} with a relative phase of {1}", Frequency, RelativePhase);

            Message msg = new Message();
            msg.Add(generateSetLevelMessage(2));

            msg.Add(generateSelectChannelMessage(2));

            msg.Add(generateSetModeMessage("singletone"));

            msg.Add(generateSelectChannelMessage(0));

            msg.Add(generateSetFrequencyMessage(Frequency));

            msg.Add(generateSelectChannelMessage(1));

            msg.Add(generateSetFrequencyMessage(Frequency));

            // The DDS has an intrinsic phase sift of pi on it
            // so the phase we have to add a phase offset of
            // rel. phase - 180
            msg.Add(generateSetPhaseMessage(RelativePhase-180));
            sendToEP2(msg);
        }
        
        #region Functions to generate messages

        private Message generateSelectChannelMessage(int ChannelNumber)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["CSR"].Address);
            msg.Add(new byte[] { (byte)(channelPatternCSR[ChannelNumber] + writePatternCSR) });
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
            
            return msg;
        }

        private Message generateSetFrequencyMessage(double Frequency)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["CFTW"].Address);
            int FTW = calculateFrequencyTuningWord(Frequency);
            msg.Add(DDSUtils.IntToMSByteArray(FTW));
            return msg;
        }

        private Message generateSetLevelMessage(int Levels)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["FR1"].Address);
            
            byte levelByte = levelPatternFR1[Levels];

            // 0xa8 and 0x20 take from Christians implementation
            msg.Add(new byte[] { 0xa8, levelByte, 0x20 });

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
            return msg;
        }

        private Message generateSetPhaseMessage(double Phase)
        {
            double moduloPhase = Phase % 360;
            if (moduloPhase < 0)
                moduloPhase = 360 - moduloPhase;
            
            Message msg = new Message();
            msg.Add(registerByShortName["CPOW"].Address);
            
            int PhaseOffsetWord = calculatePhaseOffsetWord(moduloPhase);

            byte byte1 = (byte)( (PhaseOffsetWord >> 8) &0x3f);
            byte byte2 = (byte)( PhaseOffsetWord&0xff );

            msg.Add(new byte[] { byte1, byte2 });

            return msg;
        }

        #endregion

        private void sendToEP2(Message message)
        {
            log.DebugFormat("Sending message to EP2: {0}",message.ToString());
            device.SendDataToEP2(message.ToArray());
        }


        private int calculateFrequencyTuningWord(double frequency)
        {
            return (int)(frequency * frequencyStep);
        }

        private int calculatePhaseOffsetWord(double phase)
        {
            return (int)(phase * phaseStep);
        }

        private double clockFrequency = 500e6;
        private double frequencyStep;
        private double phaseStep;
        


        #region Some byte patterns

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

        public AD9958(IDDSMicrocontroller usbDevice)
        {
            // device is protected member of the base class
            device = usbDevice;
            initializeRegisters();
            defineChannelPattern();
            defineAmpFreqPhasePattern();
            defineLevelPattern();
            defineFrequencyConstants();
            definePhaseConstants();
        }

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

    }
}
