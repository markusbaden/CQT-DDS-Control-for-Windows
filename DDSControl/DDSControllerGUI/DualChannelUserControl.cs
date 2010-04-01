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
    public partial class DualChannelUserControl : UserControl , IExtractValues
    {
        public DualChannelUserControl()
        {
            InitializeComponent();
        }

        public Dictionary<string, double> ExtractValues()
        {
            Dictionary<string, double> output = new Dictionary<string, double>();

            output.Add("frequency", Convert.ToDouble(frequencyComboBox.Text));
            output.Add("relativephase", Convert.ToDouble(phaseComboBox.Text));

            return output;
        }
    
    }
}
