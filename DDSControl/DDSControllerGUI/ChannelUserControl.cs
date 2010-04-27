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
            setting.AmplitudeScaleFactor = (int)Math.Round(Convert.ToDouble(amplitudeTextBox.Text));
            setting.Frequency = Convert.ToDouble(frequencyTextBox.Text);
            setting.Phase = Convert.ToDouble(phaseTextBox.Text);
            return setting;
        }

        public Dictionary<string, double> ExtractValues()
        {
            Dictionary<string, double> values = new Dictionary<string, double>();

            values.Add("amplitude", Convert.ToDouble(amplitudeTextBox.Text));
            values.Add("frequency", Convert.ToDouble(frequencyTextBox.Text));
            values.Add("phase", Convert.ToDouble(phaseTextBox.Text));

            return values;
        }

        private void amplitudeLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
