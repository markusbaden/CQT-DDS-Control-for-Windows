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
    public partial class ChannelUserControl : UserControl, IExtractValues
    {
        public ChannelUserControl()
        {
            InitializeComponent();
            
        }

        public Dictionary<string, double> ExtractValues()
        {
            Dictionary<string, double> values = new Dictionary<string, double>();

            values.Add("amplitude", Convert.ToDouble(amplitudeTextBox.Text));
            values.Add("frequency", Convert.ToDouble(frequencyTextBox.Text));
            values.Add("phase", Convert.ToDouble(phaseTextBox.Text));

            return values;
        }
    }
}
