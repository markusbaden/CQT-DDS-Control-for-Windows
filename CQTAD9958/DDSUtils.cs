using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DDSControl
{
    public static class DDSUtils
    {
        public static Message IntToMSByteArray(int Number)
        {
            // Get LSByteArray (LSB is at position 0)
            byte[] temp = BitConverter.GetBytes(Number);
            // Make a MSByte Msg (MSB is at position 0)
            return byteToMSBMsg(temp);
        }

        public static Message IntToMSByteArray(int Number, int NumberOfBytes)
        {
            // Get LSByte array (LSB is at position 0)
            byte[] temp = BitConverter.GetBytes(Number);

            // Extract the specified number of lsbytes
            byte[] cut = new byte[NumberOfBytes];
            for (int k = 0; k < NumberOfBytes; k++)
            {
                cut[k] = temp[k];
            }

            // Convert to MSByte message (MSB is at position 0)
            return byteToMSBMsg(cut);
        }


        private static Message byteToMSBMsg(byte[] bytes)
        {
            Message output = new Message();

            // Reorder the output of temp to adhere to the MSByte standard
            for (int k = 0; k < bytes.Length; k++)
            {
                output.Add(bytes[bytes.Length - k - 1]);
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

        public static byte Checksum(Message Msg)
        {
            int checksum = 0;
            for (int k = 0; k < Msg.Count; k++)
            {
                checksum += Msg[k];
            }
            checksum = checksum % 255;
            return (byte)checksum;
        }
    }
}
