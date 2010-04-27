using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDSControl
{
    public class ModulationSetting
    {
        public int Channel;
        public string Mode;
        public int Levels;
        public List<double> ChannelWords;
    }

    public class ChannelSetting
    {
        public int Channel;
        public int AmplitudeScaleFactor;
        public double Frequency;
        public double Phase;
    }
}
