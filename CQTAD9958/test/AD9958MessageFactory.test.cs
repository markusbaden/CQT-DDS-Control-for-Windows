using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NMock2;

namespace DDSControl
{
    public class AD9958MessageFactoryTest
    {
        private AD9958MessageFactory factory;

        [SetUp]
        public void Initialize()
        {
            factory = new AD9958MessageFactory();
        }
        [Test]
        public void TestSetModeMessageFM()
        {
            Message msg = new Message(0x03, 0x80, 0x03, 0x00);
            Assert.IsTrue(msg.Equals(factory.SetMode("fm")));
        }


        [Test]
        public void TestSetModeMessageFMLinearSweep()
        {
            Message msg = new Message(0x03, 0x80, 0x43, 0x00);
            Assert.IsTrue(msg.Equals(factory.SetMode("fm",true, false)));
        }

        [Test]
        public void TestSetModeMessageFMLinearSweepNoDwell()
        {
            Message msg = new Message(0x03, 0x80, 0xC3, 0x00);
            Assert.IsTrue(msg.Equals(factory.SetMode("fm", true, true)));
        }
        
    }
}
