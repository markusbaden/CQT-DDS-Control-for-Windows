using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DDSControl
{
    public static class DDSUtils
    {
        public static byte[] IntToMSByteArray(int Number)
        {
            byte[] temp = BitConverter.GetBytes(Number);
            byte[] output = new byte[temp.Length];

            // Reorder the output of temp to adhere to the MSByte standard
            for (int k = 0; k < temp.Length; k++)
            {
                output[k] = temp[temp.Length - k -1];
            }
            return output;
        }


        public static byte[] ConcatByteArrays(params byte[][] list)
        {
            List<byte> output = new List<byte>();
            foreach (byte[] array in list)
            {
                foreach (byte tmp in array)
                    output.Add(tmp);
            }
            return output.ToArray();
        }
    }
}
