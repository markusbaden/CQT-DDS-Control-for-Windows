using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DDSControl
{
    [TestFixture]
    public class DDSUtilsConcatTests
    {
        byte[] a;
        byte[] b;
        byte[] c;
        byte[] ab;
        byte[] aab;

        [SetUp]
        public void Intialize()
        {
            a = new byte[] { 0x0a };
            b = new byte[] { 0x0b };
            ab = new byte[] { 0x0a, 0x0b };
            aab = new byte[] { 0x0a, 0x0a, 0x0b };
        }
        
        [Test]
        public void TestConcatTwoSingleByteArrays()
        {
            byte[] concat_ab = DDSUtils.ConcatByteArrays(a, b);
            
            for (int k = 0; k < ab.Length; k++)
			{
                Assert.AreEqual(ab[k], concat_ab[k]);
			}
            
        }

        [Test]
        public void TestTwoAndOne()
        {
            byte[] concat_aab = DDSUtils.ConcatByteArrays(a, ab);

            for (int k = 0; k < aab.Length; k++)
            {
                Assert.AreEqual(aab[k], concat_aab[k]);
            }
        }

        [Test]
        public void TestThreeSingle()
        {
            byte[] concat_aab = DDSUtils.ConcatByteArrays(a, a, b);

            for (int k = 0; k < aab.Length; k++)
            {
                Assert.AreEqual(aab[k], concat_aab[k]);
            }
        }
    }
}
