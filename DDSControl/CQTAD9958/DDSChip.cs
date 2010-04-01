﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CyUSB;

namespace DDSControl
{
    public abstract class DDSChip
    {
        public abstract string Description
        {
            get;
        }

        public abstract string SerialNumber
        {
            get;
        }

        protected IDDSMicrocontroller device;
        protected Dictionary<string, DDSRegister> registerByShortName;
    
    }
}
