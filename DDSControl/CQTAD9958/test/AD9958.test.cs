using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NMock2;

namespace DDSControl
{
    [TestFixture]
    public class AD9958Tests
    {
        private AD9958 dds;
        private Mockery mocks;
        private IDDSMicrocontroller mockDevice;

        private Message setTwoLevel;
        private Message setSingleTone;
        private Message selectBothChannels;
        private Message fullDDSReset;
        
        
        [SetUp]
        public void Initialize()
        {
            mocks = new Mockery();
            mockDevice = mocks.NewMock<IDDSMicrocontroller>();
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

            // FullDDSReset via EP1
            fullDDSReset = new Message(new byte[] { 0x03, 0x08, 0x0b });
            

        }

        [Test]
        public void TestFullDDSReset()
        {
            // Define call
            Message call = new Message();
            call.Add(fullDDSReset);
            
            Expect.Once.On(mockDevice).Method("SendDataToEP1").With(call.ToArray());

            dds.FullDDSReset();

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
        private IDDSMicrocontroller mockDevice;
        private int FTW;
        private double fout;
        private byte[] CFTWCall;

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
            mockDevice = mocks.NewMock<IDDSMicrocontroller>();
            dds = new AD9958(mockDevice);
        
            // Calculate 100Mhz frequency tuning word
            double clock = 500e6;
            fout = 100e6;

            // Resolution is 2^32 which exceeds the max
            // of int32 (signed) so we use a long
            long resolution = ((long)1) << 32;

            double freqstep = resolution / clock;

            // Frequency Tuning Word FTW < 2^31 which
            // is smaller then the max of int32 so we can go back
            // to int
            FTW = (int)(fout * freqstep);

            // Calculate CFTW Call for 100Mhz
            byte[] FTWBytes = DDSUtils.IntToMSByteArray(FTW);
            CFTWCall = new byte[FTWBytes.Length + 1];
            CFTWCall[0] = 0x04;
            for (int k = 0; k < FTWBytes.Length; k++)
            {
                CFTWCall[k + 1] = FTWBytes[k];
            }

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

            // Select channel one
            selectChannelZero = new Message(new byte[] { 0x00, 0x76 });
            
            // Select channel two
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
            Expect.Once.On(mockDevice).Method("SendDataToEP2").With(CFTWCall);
            dds.SetFrequency(fout);
            mocks.VerifyAllExpectationsHaveBeenMet();
        }


        [Test]
        public void TestSetFrequencyOfChan0To100Mhz()
        {
            // Define what calls you'd expect
            // Call to channel select register with channel 0 on and open and
            // write mode MSB serial 4 bit mode
            byte[] CSRCall = new byte[] { 0x00, (byte)(0x40 + 0x36) };

            byte[] fullCall = DDSUtils.ConcatByteArrays(CSRCall, CFTWCall);

            Expect.Once.On(mockDevice).Method("SendDataToEP2").With(fullCall);
            
            dds.SetFrequency(0, fout);
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

        public void TestSetRelativePhaseOfPiHalf()
        {
            // The call sequence is (tested successfully with CyConsole)
            // 01 A8 00 20 00 F6 03 00 03 00 00 76 04 33 33 33 33 00 B6 04 33 33 33 33 00 B6 05 10 00
            // The DDS has an intrinsic relative phase shift of pi, we have to add phase offset 90-180
            // mod 2 pi
            byte[] call = new byte[] {
                0x01, 0xA8, 0x00, 0x20, // Set modulation levels to two
                0x00, 0xF6, 0x03, 0x00, 0x03, 0x00, // Select both channels, set singletone operation
                0x00, 0x76, // Select Channel 0
                0x04, 0x33, 0x33, 0x33, 0x33, // Set Freq to 100Mhz
                0x05, 0x00, 0x00, // Set Phase to zero (always as a reference)
                0x00, 0xB6, // Select Channel 1
                0x04, 0x33, 0x33, 0x33, 0x33, // Set Freq to 100 Mhz
                0x05, 0x00, 0x10 // Set Phase to (Pi-Pi)
            };

            using (mocks.Ordered)
            {
                Expect.Once.On(mockDevice).Method("SendDataToEP1").With(fullDDSReset.ToArray());
                Expect.Once.On(mockDevice).Method("SendDataToEP2").With(initialization.ToArray());
                Expect.Once.On(mockDevice).Method("SendDataToEP2").With(call.ToArray());
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
        private IDDSMicrocontroller mockMicrocontroller;

        [SetUp]
        public void Initialize()
        {
            mocks = new Mockery();
            mockMicrocontroller = mocks.NewMock<IDDSMicrocontroller>();
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
}
