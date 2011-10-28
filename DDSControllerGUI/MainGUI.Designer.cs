namespace DDSControl
{
    partial class MainGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.deviceListBox = new System.Windows.Forms.ListBox();
            this.fullResetButton = new System.Windows.Forms.Button();
            this.channelTabControl = new System.Windows.Forms.TabControl();
            this.channelZeroTabPage = new System.Windows.Forms.TabPage();
            this.channelUserControl1 = new DDSControl.ChannelUserControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dualChannelUserControl1 = new DDSControl.DualChannelUserControl();
            this.setChannelButton = new System.Windows.Forms.Button();
            this.tabControlMode = new System.Windows.Forms.TabControl();
            this.tabSingletone = new System.Windows.Forms.TabPage();
            this.tabModulation = new System.Windows.Forms.TabPage();
            this.modulationUserControl1 = new DDSControl.ModulationUserControl();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.debugDeviceListBox = new System.Windows.Forms.ListBox();
            this.button6 = new System.Windows.Forms.Button();
            this.channelTabControl.SuspendLayout();
            this.channelZeroTabPage.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabControlMode.SuspendLayout();
            this.tabSingletone.SuspendLayout();
            this.tabModulation.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // deviceListBox
            // 
            this.deviceListBox.FormattingEnabled = true;
            this.deviceListBox.Location = new System.Drawing.Point(12, 40);
            this.deviceListBox.Name = "deviceListBox";
            this.deviceListBox.Size = new System.Drawing.Size(245, 95);
            this.deviceListBox.TabIndex = 0;
            // 
            // fullResetButton
            // 
            this.fullResetButton.Location = new System.Drawing.Point(67, 184);
            this.fullResetButton.Name = "fullResetButton";
            this.fullResetButton.Size = new System.Drawing.Size(119, 23);
            this.fullResetButton.TabIndex = 10;
            this.fullResetButton.Text = "Full Reset";
            this.fullResetButton.UseVisualStyleBackColor = true;
            this.fullResetButton.Click += new System.EventHandler(this.fullResetButton_Click);
            // 
            // channelTabControl
            // 
            this.channelTabControl.Controls.Add(this.channelZeroTabPage);
            this.channelTabControl.Controls.Add(this.tabPage3);
            this.channelTabControl.Location = new System.Drawing.Point(6, 6);
            this.channelTabControl.Name = "channelTabControl";
            this.channelTabControl.SelectedIndex = 0;
            this.channelTabControl.Size = new System.Drawing.Size(261, 107);
            this.channelTabControl.TabIndex = 11;
            // 
            // channelZeroTabPage
            // 
            this.channelZeroTabPage.Controls.Add(this.channelUserControl1);
            this.channelZeroTabPage.Location = new System.Drawing.Point(4, 22);
            this.channelZeroTabPage.Name = "channelZeroTabPage";
            this.channelZeroTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.channelZeroTabPage.Size = new System.Drawing.Size(253, 81);
            this.channelZeroTabPage.TabIndex = 0;
            this.channelZeroTabPage.Text = "Single Channel";
            this.channelZeroTabPage.UseVisualStyleBackColor = true;
            // 
            // channelUserControl1
            // 
            this.channelUserControl1.Location = new System.Drawing.Point(6, 6);
            this.channelUserControl1.Name = "channelUserControl1";
            this.channelUserControl1.Size = new System.Drawing.Size(224, 88);
            this.channelUserControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dualChannelUserControl1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(253, 81);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Both Channels";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dualChannelUserControl1
            // 
            this.dualChannelUserControl1.Location = new System.Drawing.Point(6, 6);
            this.dualChannelUserControl1.Name = "dualChannelUserControl1";
            this.dualChannelUserControl1.Size = new System.Drawing.Size(241, 72);
            this.dualChannelUserControl1.TabIndex = 0;
            // 
            // setChannelButton
            // 
            this.setChannelButton.Location = new System.Drawing.Point(6, 150);
            this.setChannelButton.Name = "setChannelButton";
            this.setChannelButton.Size = new System.Drawing.Size(261, 23);
            this.setChannelButton.TabIndex = 12;
            this.setChannelButton.Text = "Set selected channel";
            this.setChannelButton.UseVisualStyleBackColor = true;
            this.setChannelButton.Click += new System.EventHandler(this.setChannelButton_Click);
            // 
            // tabControlMode
            // 
            this.tabControlMode.Controls.Add(this.tabSingletone);
            this.tabControlMode.Controls.Add(this.tabModulation);
            this.tabControlMode.Controls.Add(this.tabPage1);
            this.tabControlMode.Location = new System.Drawing.Point(272, 12);
            this.tabControlMode.Name = "tabControlMode";
            this.tabControlMode.SelectedIndex = 0;
            this.tabControlMode.Size = new System.Drawing.Size(277, 340);
            this.tabControlMode.TabIndex = 13;
            // 
            // tabSingletone
            // 
            this.tabSingletone.Controls.Add(this.channelTabControl);
            this.tabSingletone.Controls.Add(this.setChannelButton);
            this.tabSingletone.Location = new System.Drawing.Point(4, 22);
            this.tabSingletone.Name = "tabSingletone";
            this.tabSingletone.Padding = new System.Windows.Forms.Padding(3);
            this.tabSingletone.Size = new System.Drawing.Size(269, 314);
            this.tabSingletone.TabIndex = 0;
            this.tabSingletone.Text = "Singletone";
            this.tabSingletone.UseVisualStyleBackColor = true;
            // 
            // tabModulation
            // 
            this.tabModulation.Controls.Add(this.modulationUserControl1);
            this.tabModulation.Controls.Add(this.button1);
            this.tabModulation.Location = new System.Drawing.Point(4, 22);
            this.tabModulation.Name = "tabModulation";
            this.tabModulation.Padding = new System.Windows.Forms.Padding(3);
            this.tabModulation.Size = new System.Drawing.Size(269, 314);
            this.tabModulation.TabIndex = 1;
            this.tabModulation.Text = "Modulation";
            this.tabModulation.UseVisualStyleBackColor = true;
            // 
            // modulationUserControl1
            // 
            this.modulationUserControl1.Location = new System.Drawing.Point(6, 6);
            this.modulationUserControl1.Name = "modulationUserControl1";
            this.modulationUserControl1.Size = new System.Drawing.Size(243, 239);
            this.modulationUserControl1.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(52, 251);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Set Modulation";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button6);
            this.tabPage1.Controls.Add(this.debugDeviceListBox);
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(269, 314);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Debug";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(143, 35);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(126, 23);
            this.button5.TabIndex = 17;
            this.button5.Text = "FreqAmp List";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(6, 35);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(131, 23);
            this.button4.TabIndex = 16;
            this.button4.Text = "Differential Sweep List";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(143, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(131, 23);
            this.button3.TabIndex = 15;
            this.button3.Text = "Frequency List";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(131, 23);
            this.button2.TabIndex = 14;
            this.button2.Text = "Differential Sweep";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Select your DDS";
            // 
            // debugDeviceListBox
            // 
            this.debugDeviceListBox.FormattingEnabled = true;
            this.debugDeviceListBox.Location = new System.Drawing.Point(6, 213);
            this.debugDeviceListBox.Name = "debugDeviceListBox";
            this.debugDeviceListBox.Size = new System.Drawing.Size(120, 95);
            this.debugDeviceListBox.TabIndex = 18;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(7, 184);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(119, 23);
            this.button6.TabIndex = 19;
            this.button6.Text = "List of Devices";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // MainGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 370);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControlMode);
            this.Controls.Add(this.fullResetButton);
            this.Controls.Add(this.deviceListBox);
            this.Name = "MainGUI";
            this.Text = "CQT DDS Control GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainGUI_FormClosing_1);
            this.channelTabControl.ResumeLayout(false);
            this.channelZeroTabPage.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabControlMode.ResumeLayout(false);
            this.tabSingletone.ResumeLayout(false);
            this.tabModulation.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox deviceListBox;
        private System.Windows.Forms.Button fullResetButton;
        private System.Windows.Forms.TabControl channelTabControl;
        private System.Windows.Forms.TabPage channelZeroTabPage;
        private System.Windows.Forms.TabPage tabPage3;
        private ChannelUserControl channelUserControl1;
        private System.Windows.Forms.Button setChannelButton;
        private DualChannelUserControl dualChannelUserControl1;
        private System.Windows.Forms.TabControl tabControlMode;
        private System.Windows.Forms.TabPage tabSingletone;
        private System.Windows.Forms.TabPage tabModulation;
        private System.Windows.Forms.Button button1;
        private ModulationUserControl modulationUserControl1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ListBox debugDeviceListBox;

    }
}

