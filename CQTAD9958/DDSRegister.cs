using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDSControl
{
    public class DDSRegister
    {
        public string ShortName;
        public int Size;
        public byte Address;

        /// <summary>
        /// Stores the default values for a given register in a format,
        /// that is suitable for bytewise MSB transport, i.e. DefaultValues[0]
        /// is the most significant Byte.
        /// </summary>
        public byte[] DefaultValues;

        public DDSRegister(string shortName, byte address, byte[] defaultValues)
        {
            ShortName = shortName;
            Address = address;
            Size = defaultValues.Length;
            DefaultValues = defaultValues;
        }
    }
}
