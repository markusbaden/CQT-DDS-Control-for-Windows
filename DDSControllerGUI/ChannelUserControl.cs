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
    public partial class ChannelUserControl : UserControl, IChannelSetting
    {
        public ChannelUserControl()
        {
            InitializeComponent();
        }

        public ChannelSetting ChannelSetting { get {return getChannelSetting();}}

        private ChannelSetting getChannelSetting()
        {
            ChannelSetting setting = new ChannelSetting();
            setting.Channel = Convert.ToInt32(comboBox1.Text);
            setting.Frequency = Convert.ToDouble(frequencyTextBox.Text);
            return setting;
        }
    }
}
