using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DDSControl
{
    public partial class DualChannelUserControl : UserControl , IChannelSetting
    {
        public DualChannelUserControl()
        {
            InitializeComponent();
        }

        public ChannelSetting ChannelSetting { get { return getChannelSetting(); } }

        private ChannelSetting getChannelSetting()
        {
            ChannelSetting setting = new ChannelSetting();
            setting.Channel = 2;
            setting.Frequency = Convert.ToDouble(frequencyComboBox.Text);
            setting.Phase = Convert.ToDouble(phaseComboBox.Text);
            return setting;
        }
        
    
    }
}
