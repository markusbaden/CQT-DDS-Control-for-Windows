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
    public partial class ModulationUserControl : UserControl, IExtractModulationSetting
    {
        private int levels;
        private List<TextBox> channelWordBoxes = new List<TextBox>();
        private List<double> channelWords = new List<double>();

        public ModulationUserControl()
        {
            InitializeComponent();
            levels = 2;
            populateChannelWordBoxes();
        }

        public ModulationSetting ExtractModulationSetting()
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
                channelWordBoxes.Add(new TextBox());
                this.Controls.Add(channelWordBoxes[k]);
                
                System.Drawing.Point location = new Point();
                location.X = labelChannelSelection.Location.X;
                location.Y = labelChannelWords.Location.Y + k*15;
                channelWordBoxes[k].Location = location;
                channelWordBoxes[k].Size = comboBoxModulationMode.Size;                
            }
        }

    }
}
