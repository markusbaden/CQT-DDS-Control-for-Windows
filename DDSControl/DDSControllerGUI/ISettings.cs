using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDSControl
{
    public interface IExtractValues
    {
        Dictionary<string, double> ExtractValues();
    }

    public interface IModulationSetting
    {
        ModulationSetting ModulationSetting { get; }
    }

    public interface IChannelSetting
    {
        ChannelSetting ChannelSetting { get; }
    }
}
