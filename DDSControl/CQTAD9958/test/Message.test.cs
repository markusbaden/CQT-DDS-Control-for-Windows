using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DDSControl
{
    [TestFixture]
    public class TestMessage
    {
        [Test]
        public void TestSingleByteConstructor()
        {
            Message message = new Message((byte)0x0a);
            byte[] correctArray = new byte[] { 0x0a};
            byte[] messageArray = message.ToArray();

            for (int k = 0; k < correctArray.Length; k++)
            {
                Assert.AreEqual(correctArray[k], messageArray[k]);
            }
        }
        
        [Test]
        public void TestByteArrayConstructor()
        {
            Message message = new Message(new byte[] {0x0a,0x0b});
            byte[] correctArray = new byte[] { 0x0a, 0x0b };
            byte[] messageArray = message.ToArray();
            
            for(int k=0; k<correctArray.Length; k++)
            {
                Assert.AreEqual(correctArray[k], messageArray[k]);
            }
        }
        
        [Test]
        public void TestAddSingleByte()
        {
            Message message = new Message((byte)0x0a);
            message.Add(0x0b);

            byte[] correctArray = new byte[] { 0x0a, 0x0b };
            byte[] messageArray = message.ToArray();
            
            for(int k=0; k<correctArray.Length; k++)
            {
                Assert.AreEqual(correctArray[k], messageArray[k]);
            }
        }

        [Test]
        public void TestAddByteArray()
        {
            Message message = new Message((byte)0x0a);
            message.Add(new byte[] {0x0b,0x0c});

            byte[] correctArray = new byte[] { 0x0a, 0x0b, 0x0c };
            byte[] messageArray = message.ToArray();

            for (int k = 0; k < correctArray.Length; k++)
            {
                Assert.AreEqual(correctArray[k], messageArray[k]);
            }
        }

        [Test]
        public void TestAddMessage()
        {
            Message msgOne = new Message(new byte[] { 0x0a, 0x0b });
            Message msgTwo = new Message(new byte[] { 0x0c, 0x0d });
            Message msgOneTwo = new Message(new byte[] {0x0a, 0x0b, 0x0c, 0x0d });

            for (int k = 0; k < msgOne.Count; k++)
            {
                Assert.AreEqual(msgOne[k], msgOneTwo[k]);
            }

            for (int k = 0; k < msgTwo.Count; k++)
            {
                Assert.AreEqual(msgTwo[k], msgOneTwo[k+msgOne.Count]);
            }
        }

        [Test]
        public void TestToString()
        {
            Message msg = new Message(new byte[] {0x0a, 0x0b, 0x0c, 0x0d });
            string msgString = "0x0A 0x0B 0x0C 0x0D";
            Assert.AreEqual(msgString, msg.ToString());
            
        }
        
    }
}            

