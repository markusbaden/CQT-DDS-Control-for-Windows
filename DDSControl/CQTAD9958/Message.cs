using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDSControl
{
    public class Message : List<byte>
    {
        public Message() : base()
        {
        }
        
        public Message(byte Content)
        {
            this.Add(Content);
        }
        
        public Message(params byte[] Content)
        {
            this.AddRange(Content);
        }

        public void Add(byte[] Array)
        {
                this.AddRange(Array);
        }

        public void Add(Message message)
        {
            this.AddRange(message);
        }

        public bool Equals(Message msg)
        {
            bool ret = true;
            if (msg.Count!=this.Count)
            {
                ret = false;
            }
            else
            {
                for (int k = 0; k < msg.Count; k++)
                {
                    if (msg[k] != this[k])
                    {
                        ret = false;
                    }
                }
            }
            return ret;
        }

        public override string ToString()
        {
            StringBuilder returnString = new StringBuilder();
            for (int i=0; i<this.Count ; i++)
            {
                returnString.Append("0x"+ this[i].ToString("X2"));
                if (i < Count - 1)
                {
                    returnString.Append(" ");
                }
            }
            return returnString.ToString();
        }

    }
}
