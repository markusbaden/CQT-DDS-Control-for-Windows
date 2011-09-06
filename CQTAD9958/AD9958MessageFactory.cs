using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDSControl
{
    public class AD9958MessageFactory
    {
        #region log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion //log4net

        private double clockFrequency = 500e6;
        private double syncFrequency 
        {
            get { return clockFrequency / 4; }
        }
        private double frequencyStep;
        private double phaseStep;
        private double rampStep;
        private Dictionary<string, DDSRegister> registerByShortName;


        #region Functions to generate messages

        public Message SelectChannel(int ChannelNumber)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["CSR"].Address);
            msg.Add(new byte[] { (byte)(channelPatternCSR[ChannelNumber] + writePatternCSR) });
            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to select channel {0}: {1}", ChannelNumber, msg); }
            return msg;
        }

        public Message SetChannelWord(int ChannelRegisterNumber, byte[] Content)
        {
            Message msg = new Message();
            byte addressByte = (byte)(0x0A + (ChannelRegisterNumber - 1));
            msg.Add(addressByte);
            msg.Add(Content);

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set Channel Register {0} to given content: {1}", ChannelRegisterNumber, msg); }

            return msg;
        }

        public Message SetAmplitude(int AmplitudeScaleFactor)
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

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set current amplitude to scalefac {0}: {1}", AmplitudeScaleFactor, msg); }
            return msg;
        }

        public Message SetFrequency(double Frequency)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["CFTW"].Address);
            msg.Add(FrequencyMessage(Frequency));
            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set current frequency to {0:0.000e0}: {1}", Frequency, msg); }
            return msg;
        }

        public Message FrequencyMessage(double Frequency)
        {
            Message msg = new Message();
            msg.Add(calculateFrequencyTuningWordAsBytes(Frequency));
            if (log.IsDebugEnabled) { log.DebugFormat("Generated message for a frequency of to {0:0.000e0}: {1}", Frequency, msg); }
            return msg;
        }

        public Message SetLevel(int Levels)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["FR1"].Address);

            byte levelByte = levelPatternFR1[Levels];

            // 0xa8 and 0x20 take from Christians implementation
            msg.Add(new byte[] { 0xa8, levelByte, 0x20 });

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set levels to {0}: {1}", Levels, msg); }

            return msg;
        }

        public Message SetMode(string Mode)
        {
            return SetMode(Mode, false, false);
        }

        public Message SetMode(string Mode, bool LinearSweepEnable, bool LinearSweepNoDwell)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["CFR"].Address);

            // CFR part
            // The first byte just determines the mode (i.e. AM/FM/PM)
            byte ampFreqPhaseByte = ampFreqPhasePatternCFR[Mode];

            // The second byte sets the DAC current, the Load SRR at I/0 update
            // and most importantly the linear sweep no dwell and linear sweep enable
            // bits, the standard byte is 0x03 where linear sweep enable is 0
            // and linear sweep no dwell as well
            byte byte2 = 0x03;
            

            // If LinearSweepEnable is set we have to set bit 6 of that byte to one
            // first mask bits 5:0 by a bitwise and with 11 1111 = 0x3f and then
            // do a bitwise XOR with 100 0000 = 0x40
            if (LinearSweepEnable)
            {
                byte2 = (byte)((byte2 & 0x3f) ^ 0x40);
            }
            
            // If LinearSweepNoDwell is set we have set bit 7 of that byte to 1
            // We first mask bits 6:0 by an bitwise and with 111 1111 = 0x7F
            // and then prepend a 1 by an bitwise XOR with 1000 000 = 0x80
            if (LinearSweepNoDwell)
            {
                byte2 = (byte)((byte2 & 0x7f) ^ 0x80);
            }

            // 0x00 taken from Christian's implementation
            msg.Add(new byte[] { ampFreqPhaseByte, byte2, 0x00 });

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set mode to {0} with linear sweep {2} and  no dwell {3}: {1}", Mode, msg,LinearSweepEnable,LinearSweepNoDwell); }

            return msg;
        }

        public Message FillChannelWords(string ModulationType, params double[] ChannelWordList)
        {
            Message msg = new Message();
            switch (ModulationType)
            {
                case "fm":
                    msg.Add(SetFrequency(ChannelWordList[0]));
                    msg.Add(SetChannelWord(1, FrequencyMessage(ChannelWordList[1]).ToArray()));
                    break;
                case "pm":
                    msg.Add(SetPhase(ChannelWordList[0]));
                    msg.Add(SetChannelWord(1, PhaseAsChannelWord(ChannelWordList[1]).ToArray()));
                    break;
                default:
                    if (log.IsErrorEnabled) { log.ErrorFormat("Could not recognize the modulatioin type {0} while trying to fill in channel words", ModulationType); }
                    break;
            }
            return msg;
        }

        public Message SetPhase(double Phase)
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
            byte byte1 = (byte)((PhaseOffsetWord >> 8) & 0x3f);

            // In order to store bits 7:0 we first blank out the higher bits by doing a bitwise
            // and with 1111 1111 = 0xFF and then store it
            byte byte2 = (byte)(PhaseOffsetWord & 0xff);

            msg.Add(new byte[] { byte1, byte2 });

            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set current phase to {0}: {1}", moduloPhase, msg); }

            return msg;
        }

        public Message PhaseAsChannelWord(double Phase)
        {
            // The Channel Word Register are 4byte long and the PhaseOffsetWord only 14bit long
            // the PhaseOffsetWord has to be MSB aligned in the register
            int PhaseOffsetWord = calculatePhaseOffsetWord(Phase);

            // First shift the word by two bits to the left to make it MSB aligned
            // for a two byte register
            PhaseOffsetWord = PhaseOffsetWord << 2;

            // Then convert it to MSBytearray
            Message msg = DDSUtils.IntToMSByteArray(PhaseOffsetWord, 2);

            // Finally make sure that the two last bits are zero by doing a
            // bitwise and with 1111 1100 = 0xFC
            msg[1] = (byte)(msg[1] & 0xFC);

            // Add to zero bytes for the remaining byte registers
            msg.AddRange(new byte[] { 0x00, 0x00 });

            return msg;
        }

        public Message SetRampRate(double RampRate)
        {
            return SetRampRate(RampRate, RampRate);
        }

        public Message SetRampRate(double RisingRampRate, double FallingRampRate)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["LSRR"].Address);
            msg.Add(calculateRampRateWord(RisingRampRate));
            msg.Add(calculateRampRateWord(FallingRampRate));
            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set rising ramp rate to {0} and falling ramp rate to {1}: {2}", RisingRampRate,FallingRampRate,msg); }
            return msg;
        }

        public Message SetDeltaFrequency(double DeltaFrequency)
        {
            Message msg = new Message();
            msg.Add(SetRisingDeltaFrequency(DeltaFrequency));
            msg.Add(SetFallingDeltaFrequency(DeltaFrequency));
            return msg;
        }

        public Message SetRisingDeltaFrequency(double RisingDeltaFrequency)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["RDW"].Address);
            msg.Add(FrequencyMessage(RisingDeltaFrequency));
            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set rising delta frequency to {0}: {1}", RisingDeltaFrequency, msg); }
            return msg;
        }

        public Message SetFallingDeltaFrequency(double FallingDeltaFrequency)
        {
            Message msg = new Message();
            msg.Add(registerByShortName["FDW"].Address);
            msg.Add(FrequencyMessage(FallingDeltaFrequency));
            if (log.IsDebugEnabled) { log.DebugFormat("Generated message to set falling delta frequency to {0}: {1}", FallingDeltaFrequency, msg); }
            return msg;
        }

        #endregion


        #region Functions for calculating words

        private int calculateFrequencyTuningWord(double frequency)
        {
            return (int)(Math.Round(frequency * frequencyStep));
        }

        private byte[] calculateFrequencyTuningWordAsBytes(double frequency)
        {
            int FTW = calculateFrequencyTuningWord(frequency);
            return DDSUtils.IntToMSByteArray(FTW).ToArray();
        }

        private int calculatePhaseOffsetWord(double phase)
        {
            return (int)(Math.Round(phase * phaseStep));
        }

        private double calculateModuloPhase(double Phase)
        {
            double moduloPhase = Phase % 360;
            if (moduloPhase < 0)
                moduloPhase = moduloPhase + 360;
            return moduloPhase;
        }

        private byte[] calculateRampRateWord(double RampRate)
        {
            // Calculate the the 8bit word. since it is assumed
            // to be nonzero a 0 is a one (and a 255 a 256)
            // that's why we substract one from the result. Cheers
            int word = (int) (Math.Round(RampRate * rampStep)) - 1;
            // Int has 4 bytes, but Ramp Rate word only 1
            return DDSUtils.IntToMSByteArray(word,1).ToArray();
        }

        #endregion

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

        public AD9958MessageFactory()
        {
            initializeRegisters();
            defineChannelPattern();
            defineAmpFreqPhasePattern();
            defineLevelPattern();
            defineConstants();
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
            addRegister("CFR", 0x03, new byte[] { 0x00, 0x03, 0x00 });

            // Channel Frequency Tuning Word (CFTW) Register 
            addRegister("CFTW", 0x04, new byte[] { 0x00, 0x00, 0x00, 0x00 });

            // Channel Phase Offset Word (CPOW) Register
            addRegister("CPOW", 0x05, new byte[] { 0x00, 0x00 });

            // Amplitude Control Regist (ACR)
            addRegister("ACR", 0x06, new byte[] { 0x00, 0x00, 0x00 });

            // Linear Sweep Ramp Rate
            addRegister("LSRR", 0x07, new byte[] { 0x00, 0x00 });

            // Linear Sweep Ramp Rising Delta Word
            addRegister("RDW", 0x08, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        
            // Linear Sweep Ramp Falling Delta Word
            addRegister("FDW", 0x09, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        }

        private void addRegister(string Name, byte Address, byte[] DefaultValues)
        {
            registerByShortName.Add(Name, new DDSRegister(Name, Address, DefaultValues));
        }

        private void defineChannelPattern()
        {
            channelPatternCSR = new Dictionary<int, byte>();
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

        private void defineConstants()
        {
            // Fout = (FrequencyTuningWord * clockFrequency) / (2^32)
            // <=> FTW = Fout * frequencyStep
            // And use long for the calculation because int is int32 < 2^31
            frequencyStep = (((long)1 << 32) / clockFrequency);

            // PhaseOffset = (PhaseOffsetWord/(2^14)) * 360
            // <=> POW = PhaseOffset * phaseStep
            phaseStep = Math.Pow(2, 14) / 360;

            // RampRate = (RSRR) * 1/sync_clock
            // <=> RSRR = RampRate * rampStep
            rampStep = syncFrequency;
        }

        #endregion
        
    }
}
