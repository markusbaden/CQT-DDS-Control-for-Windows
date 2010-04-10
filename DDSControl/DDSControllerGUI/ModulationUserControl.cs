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
    public partial class ModulationUserControl : UserControl, IModulationSetting
    {
        private int levels;
        private List<NumberedTextBox> channelWordBoxes = new List<NumberedTextBox>();
        private List<double> channelWords = new List<double>();

        public ModulationSetting ModulationSetting { get { return extractModulationSetting(); } }

        public ModulationUserControl()
        {
            InitializeComponent();
            levels = 2;
            populateChannelWordBoxes();
        }

        private ModulationSetting extractModulationSetting()
        {
            ModulationSetting output = new ModulationSetting();
            output.Mode = comboBoxModulationMode.Text;
            output.Levels = levels;
            output.ChannelWords = channelWords;
            output.Channel = 0;
            return output;
        }

        private void populateChannelWordBoxes()
        {
            for (int k = 0; k < levels; k++)
            {
                channelWordBoxes.Add(new NumberedTextBox());
                this.Controls.Add(channelWordBoxes[k]);
                
                System.Drawing.Point location = new Point();
                location.X = labelChannelSelection.Location.X;
                location.Y = labelChannelWords.Location.Y + k*25;
                channelWordBoxes[k].Location = location;
                channelWordBoxes[k].Size = comboBoxModulationMode.Size;
                channelWordBoxes[k].Number = k;
                channelWordBoxes[k].Text = "0";
                channelWords.Add(0);
                channelWordBoxes[0].TextChanged += new EventHandler(handleChannelWordChange);
            }
        }

        public void handleChannelWordChange(object sender, EventArgs e)
        {
            NumberedTextBox senderbox = (NumberedTextBox)sender;
            channelWords[senderbox.Number] = Convert.ToDouble(senderbox.Text);
        }

    }
}
