using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NMock2;

namespace DDSControl
{
    [TestFixture]
    public class TestBasicCommands
    {
        private AD9958 dds;
        private Mockery mocks;
        private IDDSUSBChip mockDevice;

        private Message setTwoLevel;
        private Message setSingleTone;
        private Message selectBothChannels;
        private Message fullDDSReset;
        private Message startTransfer;
        private Message stopTransfer;
        private Message listPlayMode10bytes;
        private Message startListPlayMode;
        private Message stopListPlayMode;
        
        [SetUp]
        public void Initialize()
        {
            mocks = new Mockery();
            mockDevice = mocks.NewMock<IDDSUSBChip>();
            dds = new AD9958(mockDevice);

            // Define some common calls

            // Set to Two Level Modulation
            // Call to Function Register 1 according to Christian's implementation
            setTwoLevel = new Message(new byte[] { 0x01, 0xa8, 0x00, 0x20 });

            // Set to single tone
            // Call to channel function register with AFP select none, 
            // the middle byte to default and the LSByte to all zeros
            // as in Christians code
            setSingleTone = new Message(new byte[] { 0x03, 0x00, 0x03, 0x00 });

            // Select both channels
            // Call to channel select register with both channels on and open and
            // write mode MSB serial 4 bit mode
            selectBothChannels = new Message(new byte[] { 0x00, (byte)(0xc0 + 0x36) });

            // Full_DDS_Reset via EP1
            fullDDSReset = new Message(new byte[] { 0x03, 0x08, 0x0b });

            // Start_Transfer via EP1
            startTransfer = new Message(0x03, 0x03, 0x06);
        
            // Stop Transfer via EP1
            stopTransfer = new Message(0x03, 0x04, 0x07);
        
            // ListplayMode via EP1 (10 byte length)
            listPlayMode10bytes = new Message(0x04, 0x0b, 0x0a, 0x19);

            // StartListplayMode via EP1
            startListPlayMode = new Message(0x03, 0x0c, 0x0f);
            
            // StopListplay mode via EP1
            stopListPlayMode = new Message(0x03, 0x0e, 0x11);
        
        }

        [Test]
        public void TestFullDDSReset()
        {
            // Define call
            Message call = new Message();
            call.Add(fullDDSReset);
            Expect.Once.On(mockDevice).Method("SendDataToEP1").With(call.ToArray());
            dds.Full_DDS_Reset();
            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestStartTransfer()
        {
            Message call = new Message();
            call.Add(startTransfer);
            Expect.Once.On(mockDevice).Method("SendDataToEP1").With(call.ToArray());
            dds.Start_Transfer();
            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestStopTransfer()
        {
            Message call = new Message();
            call.Add(stopTransfer);
            Expect.Once.On(mockDevice).Method("SendDataToEP1").With(call.ToArray());
            dds.Stop_Transfer();
            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestListplayMode()
        {
            Message call = new Message();
            call.Add(listPlayMode10bytes);
            Expect.Once.On(mockDevice).Method("SendDataToEP1").With(call.ToArray());
            dds.ListplayMode(10);
            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestStartListplaymode()
        {
            Message call = new Message();
            call.Add(startListPlayMode);
            Expect.Once.On(mockDevice).Method("SendDataToEP1").With(call.ToArray());
            dds.StartListplayMode();
            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestStopListplayMode()
        {
            Message call = new Message();
            call.Add(stopListPlayMode);
            Expect.Once.On(mockDevice).Method("SendDataToEP1").With(call.ToArray());
            dds.StopListplayMode();
            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestMasterReset()
        {
            Message fullResetCall = new Message();
            fullResetCall.Add(fullDDSReset);

            Message initializeCall = new Message();
            initializeCall.Add(selectBothChannels);
            initializeCall.Add(setSingleTone);
            initializeCall.Add(setTwoLevel);

            using (mocks.Ordered)
            {
                Expect.Once.On(mockDevice).Method("SendDataToEP1").With(fullResetCall.ToArray());
                Expect.Once.On(mockDevice).Method("SendDataToEP2").With(initializeCall.ToArray());
            }

            dds.MasterReset();

            mocks.VerifyAllExpectationsHaveBeenMet();
        }
        
        [Test]
        public void TestSetSingleToneMode()
        {

            // Define what message you expect
            Message call = new Message();
            call.Add(selectBothChannels);
            call.Add(setSingleTone);

            using (mocks.Ordered)
            {
                Expect.Once.On(mockDevice).Method("SendDataToEP2").With(call.ToArray());
            }

            dds.SetMode("singletone");

            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestSetTwoLevels()
        {
            Expect.Once.On(mockDevice).Method("SendDataToEP2").With(setTwoLevel.ToArray());
            dds.SetLevels(2);
            mocks.VerifyAllExpectationsHaveBeenMet();
            
        }
    }

    [TestFixture]
    public class TestFrequencyCommands
    {
        private AD9958 dds;
        private Mockery mocks;
        private IDDSUSBChip mockDevice;

        private Message fullDDSReset;
        private Message setTwoLevel;
        private Message setSingleTone;
        private Message selectBothChannels;
        private Message selectChannelZero;
        private Message selectChannelOne;
        private Message setFreqTo100MHz;
        private Message setPhaseToZero;
        private Message initialization;

        [SetUp]
        public void Initialize()
        {
            mocks = new Mockery();
            mockDevice = mocks.NewMock<IDDSUSBChip>();
            dds = new AD9958(mockDevice);

            // Define some messages
            // FullDDSReset via EP1
            fullDDSReset = new Message(new byte[] { 0x03, 0x08, 0x0b });

            // Set to Two Level Modulation
            // Call to Function Register 1 according to Christian's implementation
            setTwoLevel = new Message(new byte[] { 0x01, 0xa8, 0x00, 0x20 });

            // Set to single tone
            // Call to channel function register with AFP select none, 
            // the middle byte to default and the LSByte to all zeros
            // as in Christians code
            setSingleTone = new Message(new byte[] { 0x03, 0x00, 0x03, 0x00 });

            // Select both channels
            // Call to channel select register with both channels on and open and
            // write mode MSB serial 4 bit mode
            selectBothChannels = new Message(new byte[] { 0x00, (byte)(0xc0 + 0x36) });

            // Select channel zero
            selectChannelZero = new Message(new byte[] { 0x00, 0x76 });
            
            // Select channel one
            selectChannelOne = new Message(new byte[] { 0x00, 0xB6 });

            // Set frequency to 100 MHz
            // 0x33 0x33 0x33 0x33 / 2**32 = 0.2
            setFreqTo100MHz = new Message(new byte[] { 0x04, 0x33, 0x33, 0x33, 0x33 });

            // Set phase to zero
            setPhaseToZero = new Message(new byte[] { 0x05, 0x00, 0x00 });
            
            // Initialization after MasterReset
            initialization = new Message();
            initialization.Add(selectBothChannels);
            initialization.Add(setSingleTone);
            initialization.Add(setTwoLevel);
        }
        [Test]
        public void TestSetFrequencyTo100Mhz()
        {
            Message call = new Message();
            call.Add(setFreqTo100MHz);
            Expect.Once.On(mockDevice).Method("SendDataToEP2").With(call.ToArray());
            dds.SetFrequency(100e6);
            mocks.VerifyAllExpectationsHaveBeenMet();
        }


        [Test]
        public void TestSetFrequencyOfChan0To100Mhz()
        {
            Message call = new Message();
            call.Add(selectChannelZero);
            call.Add(setFreqTo100MHz);

            Expect.Once.On(mockDevice).Method("SendDataToEP2").With(call.ToArray());
            
            dds.SetFrequency(0, 100e6);
            mocks.VerifyAllExpectationsHaveBeenMet();
        }


        [Test]
        public void TestSetRelativePhaseOfPi()
        {
            // Set both channels to 100 MHz and a relative phase zero since
            // there is an additional phase shift of pi on the board
            Message setChannelsCall = new Message();

            setChannelsCall.Add(setTwoLevel); // Set modulation levels to two
            setChannelsCall.Add(selectBothChannels); // Select both channels
            setChannelsCall.Add(setSingleTone); // Set singletone mode
            setChannelsCall.Add(selectChannelZero); // Select channel 0
            setChannelsCall.Add(setFreqTo100MHz); // Set freq to 100MHz
            setChannelsCall.Add(setPhaseToZero); // Set phase to zero (always as reference)
            setChannelsCall.Add(selectChannelOne); // Select channel 1
            setChannelsCall.Add(setFreqTo100MHz); // Set freq to 100 Mhz
            setChannelsCall.Add(setPhaseToZero); // Set phase to zero (since there is a phase shift of pi on the pcb)

            using (mocks.Ordered)
            {
                Expect.Once.On(mockDevice).Method("SendDataToEP1").With(fullDDSReset.ToArray());
                Expect.Once.On(mockDevice).Method("SendDataToEP2").With(initialization.ToArray());
                Expect.Once.On(mockDevice).Method("SendDataToEP2").With(setChannelsCall.ToArray());
            }

            dds.SetTwoChannelRelativePhase(100e6, 180);
            
            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestSetRelativePhaseOfPiHalf()
        {

            // Set both channels to 100 MHz and a relative phase zero since
            // there is an additional phase shift of pi on the board
            Message setChannelsCall = new Message();

            setChannelsCall.Add(setTwoLevel); // Set modulation levels to two
            setChannelsCall.Add(selectBothChannels); // Select both channels
            setChannelsCall.Add(setSingleTone); // Set singletone mode
            setChannelsCall.Add(selectChannelZero); // Select channel 0
            setChannelsCall.Add(setFreqTo100MHz); // Set freq to 100MHz
            setChannelsCall.Add(setPhaseToZero); // Set phase to zero (always as reference)
            setChannelsCall.Add(selectChannelOne); // Select channel 1
            setChannelsCall.Add(setFreqTo100MHz); // Set freq to 100 Mhz
            setChannelsCall.Add(new byte[] {0x05, 0x30, 0x00} ); // Set phase to 270 (since there is a phase shift of pi on the pcb)

            using (mocks.Ordered)
            {
                Expect.Once.On(mockDevice).Method("SendDataToEP1").With(fullDDSReset.ToArray());
                Expect.Once.On(mockDevice).Method("SendDataToEP2").With(initialization.ToArray());
                Expect.Once.On(mockDevice).Method("SendDataToEP2").With(setChannelsCall.ToArray());
            }

            dds.SetTwoChannelRelativePhase(100e6, 90);

            mocks.VerifyAllExpectationsHaveBeenMet();
        }
    }

    [TestFixture]
    public class TestAmplitudeCommands
    {
        private AD9958 dds;
        private Mockery mocks;
        private IDDSUSBChip mockMicrocontroller;

        [SetUp]
        public void Initialize()
        {
            mocks = new Mockery();
            mockMicrocontroller = mocks.NewMock<IDDSUSBChip>();
            dds = new AD9958(mockMicrocontroller);
        }

        [Test]
        public void TestSetFullAmplitudeScale()
        {
            // Define call to Amplitude Control Register (ACR)
            // We want to set the amplitude manually, so ACR[12]=1
            // and ACR[11]=0, the bits ACR[9:0] define the amplitude
            // scale factor, which we want to set to max (so all 1)
            // the remaining bits ACR[23:13,10] have no effect in manual
            // mode, so we just set them to 0, this results in the call
            // ACR address = 0x06
            // ACR[23:16] = 00000000 = 0x00
            // ACR[15:08] = 00010011 = 0x13
            // ACR[07:00] = 11111111 = 0xFF

            byte[] acrCall = { 0x06, 0x00, 0x13, 0xFF };

            Expect.Once.On(mockMicrocontroller).Method("SendDataToEP2").With(acrCall);

            dds.SetAmplitude(1023);

            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestSetNonFullScale()
        {
            // We use 0100000000 = 256 instead of full scale
            // and 0001110010 = 114 as test cases for the rest
            // of the byte see comment in TestSetFullAmplitudeScale
            // (2 cases so that we can have faith in the splitting of 
            // the amplitude scale factor over multiple register bytes)

            int firstAmplitude = 256;
            byte[] firstACRCall = new byte[] { 0x06, 0x00, 0x11, 0x00 };

            int secondAmplitude = 114;
            byte[] secondACRCall = new byte[] { 0x06, 0x00, 0x10, 0x72 };

            using (mocks.Ordered)
            {
                Expect.Once.On(mockMicrocontroller).Method("SendDataToEP2").With(firstACRCall);
                Expect.Once.On(mockMicrocontroller).Method("SendDataToEP2").With(secondACRCall);

            }

            dds.SetAmplitude(firstAmplitude);
            dds.SetAmplitude(secondAmplitude);

            mocks.VerifyAllExpectationsHaveBeenMet();

        }
    }

    [TestFixture]
    public class TestModulation
    {
        private AD9958 dds;
        private Mockery mocks;
        private IDDSUSBChip mockMicrocontroller;
        
        private Message selectChannelZero;
        private Message setTwoLevel;
        private Message selectFrequencyModulation;
        private Message selectPhaseModulation;        
        private Message setFreqTuningWord1MHz;
        private Message setChanWordOne2MHz;
        private Message setPhaseTuningWordZero;
        private Message setChanWordOnePi;

        [SetUp]
        public void Initialize()
        {
            mocks = new Mockery();
            mockMicrocontroller = mocks.NewMock<IDDSUSBChip>();
            dds = new AD9958(mockMicrocontroller);

            // Define some messages
            // Select channel zero
            selectChannelZero = new Message(new byte[] { 0x00, 0x76 });

            // Set to Two Level Modulation
            // Call to Function Register 1 according to Christian's implementation
            setTwoLevel = new Message(new byte[] { 0x01, 0xa8, 0x00, 0x20 });

            // Set to frequency modulation
            // Call to channel function register with AFP select FM, 
            // the middle byte to default and the LSByte to all zeros
            // as in Christians code
            selectFrequencyModulation = new Message(new byte[] { 0x03, 0x80, 0x03, 0x00 });

            // Set frequency tuning word of current channel to 1 MHz
            // 0x00 0x83 0x12 0x6E / 2**32 = 0.002
            setFreqTuningWord1MHz = new Message(new byte[] { 0x04, 0x00, 0x83, 0x12, 0x6F });

            // Set Channel Word Register 1 to 2 MHz
            setChanWordOne2MHz = new Message(new byte[] { 0x0A, 0x01, 0x06, 0x24, 0xDD });

            // Set to phase modulation
            // Call to channel function register with AFP select PM, 
            // the middle byte to default and the LSByte to all zeros
            // as in Christians code
            selectPhaseModulation = new Message(new byte[] { 0x03, 0xC0, 0x03, 0x00 });
            
            // Set phase tuning word to zero
            setPhaseTuningWordZero = new Message(new byte[] { 0x05, 0x00, 0x00 });
            
            // Set channel register word 1 to pi (resolution 14bit)
            // Pi is 2**13 (10 0000 0000) we have to MSB align it in the 32 bit CW1 register
            // where we set all others to zero
            setChanWordOnePi = new Message(new byte[] { 0x0A, 0x80, 0x00, 0x00, 0x00 });
        }

        [Test]
        public void TestSetSingleChannelFM()
        {
            Message call = new Message();
            call.Add(selectChannelZero);
            call.Add(setTwoLevel);
            call.Add(selectFrequencyModulation);
            call.Add(setFreqTuningWord1MHz);
            call.Add(setChanWordOne2MHz);

            Expect.Once.On(mockMicrocontroller).Method("SendDataToEP2").With(call.ToArray());
            dds.SetModulation(0, 2, "fm", 1e6, 2e6);

            mocks.VerifyAllExpectationsHaveBeenMet();
            
        }

        [Test]
        public void TestPhaseModulation()
        {
            Message call = new Message();
            call.Add(selectChannelZero);
            call.Add(setTwoLevel);
            call.Add(selectPhaseModulation);
            call.Add(setPhaseTuningWordZero);
            call.Add(setChanWordOnePi);

            Expect.Once.On(mockMicrocontroller).Method("SendDataToEP2").With(call.ToArray());

            dds.SetModulation(0, 2, "pm", 0, 180);
            
            mocks.VerifyAllExpectationsHaveBeenMet();
        
        }
    }

    [TestFixture]
    public class TestLinearSweep
    {
        private AD9958 dds;
        private Mockery mocks;
        private IDDSUSBChip mockMicrocontroller;

        // Select channel 0
        private Message selectChannelZero = new Message(0x00, 0x76);
        
        // Call FR1, PLL=5, Modulation Level =2, sync clock disable
        private Message setTwoLevel = new Message(0x01, 0xa8, 0x00, 0x20);
        
        // Call CFR, modulation mode fm, linear sweep enable, clear phase accumulator 0
        private Message setFMLinearSweep = new Message(0x03, 0x80, 0x43, 0x00);
        
        // Call CFTW witch 1/500 * 2^32 = 82589936
        private Message setStartfrequency1MHz = new Message(0x04, 0x00, 0x83, 0x12, 0x6f);
        
        // Call CW1 with 2/500 * 2^32 = 17179872
        private Message setEndFrequency2Mhz = new Message(0x0a, 0x01, 0x06, 0x24, 0xdd);
        
        // Call RDW with (1e3 / 500e6) * 2^32= 8590
        private Message setRDW1kHz = new Message(0x08, 0x00, 0x00, 0x21, 0x8e);
        
        // Call FDW with (1e3 / 500e6) * 2^32 = 8590
        private Message setFDW1kHz = new Message(0x09, 0x00, 0x00, 0x21, 0x8e);
        
        // Call RSRR with both 1.024e-6*125e6 * 256 = 128
        private Message setRSRR1us =new Message(0x07, 0x80, 0x80);

        [SetUp]
        public void Initialize()
        {
            mocks = new Mockery();
            mockMicrocontroller = mocks.NewMock<IDDSUSBChip>();
            dds = new AD9958(mockMicrocontroller);

        }

        [Test]
        public void TestLinearUpSweep()
        {
            
            Message msg = new Message();

            msg.Add(selectChannelZero);
            msg.Add(setTwoLevel);
            msg.Add(setFMLinearSweep);
            msg.Add(setStartfrequency1MHz);
            msg.Add(setEndFrequency2Mhz);
            msg.Add(setRSRR1us);
            msg.Add(setRDW1kHz);
            msg.Add(setFDW1kHz);

            Expect.Once.On(mockMicrocontroller).Method("SendDataToEP2").With(msg.ToArray());
            dds.SetLinearSweep(0, 1e6, 2e6, 1.024e-6, 1e3);
            mocks.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
