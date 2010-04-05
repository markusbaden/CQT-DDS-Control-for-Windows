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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.setChannelButton = new System.Windows.Forms.Button();
            this.tabControlMode = new System.Windows.Forms.TabControl();
            this.tabSingletone = new System.Windows.Forms.TabPage();
            this.tabModulation = new System.Windows.Forms.TabPage();
            this.channelUserControl1 = new DDSControl.ChannelUserControl();
            this.channelUserControl2 = new DDSControl.ChannelUserControl();
            this.dualChannelUserControl1 = new DDSControl.DualChannelUserControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.channelTabControl.SuspendLayout();
            this.channelZeroTabPage.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabControlMode.SuspendLayout();
            this.tabSingletone.SuspendLayout();
            this.tabModulation.SuspendLayout();
            this.SuspendLayout();
            // 
            // deviceListBox
            // 
            this.deviceListBox.FormattingEnabled = true;
            this.deviceListBox.Location = new System.Drawing.Point(12, 12);
            this.deviceListBox.Name = "deviceListBox";
            this.deviceListBox.Size = new System.Drawing.Size(245, 95);
            this.deviceListBox.TabIndex = 0;
            // 
            // fullResetButton
            // 
            this.fullResetButton.Location = new System.Drawing.Point(263, 12);
            this.fullResetButton.Name = "fullResetButton";
            this.fullResetButton.Size = new System.Drawing.Size(75, 23);
            this.fullResetButton.TabIndex = 10;
            this.fullResetButton.Text = "Full Reset";
            this.fullResetButton.UseVisualStyleBackColor = true;
            this.fullResetButton.Click += new System.EventHandler(this.fullResetButton_Click);
            // 
            // channelTabControl
            // 
            this.channelTabControl.Controls.Add(this.channelZeroTabPage);
            this.channelTabControl.Controls.Add(this.tabPage2);
            this.channelTabControl.Controls.Add(this.tabPage3);
            this.channelTabControl.Location = new System.Drawing.Point(6, 6);
            this.channelTabControl.Name = "channelTabControl";
            this.channelTabControl.SelectedIndex = 0;
            this.channelTabControl.Size = new System.Drawing.Size(261, 138);
            this.channelTabControl.TabIndex = 11;
            // 
            // channelZeroTabPage
            // 
            this.channelZeroTabPage.Controls.Add(this.channelUserControl1);
            this.channelZeroTabPage.Location = new System.Drawing.Point(4, 22);
            this.channelZeroTabPage.Name = "channelZeroTabPage";
            this.channelZeroTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.channelZeroTabPage.Size = new System.Drawing.Size(253, 112);
            this.channelZeroTabPage.TabIndex = 0;
            this.channelZeroTabPage.Text = "Channel 0";
            this.channelZeroTabPage.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.channelUserControl2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(253, 112);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Channel 1";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dualChannelUserControl1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(253, 112);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Both";
            this.tabPage3.UseVisualStyleBackColor = true;
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
            this.tabControlMode.Location = new System.Drawing.Point(386, 12);
            this.tabControlMode.Name = "tabControlMode";
            this.tabControlMode.SelectedIndex = 0;
            this.tabControlMode.Size = new System.Drawing.Size(388, 340);
            this.tabControlMode.TabIndex = 13;
            // 
            // tabSingletone
            // 
            this.tabSingletone.Controls.Add(this.channelTabControl);
            this.tabSingletone.Controls.Add(this.setChannelButton);
            this.tabSingletone.Location = new System.Drawing.Point(4, 22);
            this.tabSingletone.Name = "tabSingletone";
            this.tabSingletone.Padding = new System.Windows.Forms.Padding(3);
            this.tabSingletone.Size = new System.Drawing.Size(380, 314);
            this.tabSingletone.TabIndex = 0;
            this.tabSingletone.Text = "Singletone";
            this.tabSingletone.UseVisualStyleBackColor = true;
            // 
            // tabModulation
            // 
            this.tabModulation.Controls.Add(this.label6);
            this.tabModulation.Controls.Add(this.label5);
            this.tabModulation.Controls.Add(this.label4);
            this.tabModulation.Controls.Add(this.label3);
            this.tabModulation.Controls.Add(this.label2);
            this.tabModulation.Controls.Add(this.label1);
            this.tabModulation.Location = new System.Drawing.Point(4, 22);
            this.tabModulation.Name = "tabModulation";
            this.tabModulation.Padding = new System.Windows.Forms.Padding(3);
            this.tabModulation.Size = new System.Drawing.Size(380, 314);
            this.tabModulation.TabIndex = 1;
            this.tabModulation.Text = "Modulation";
            this.tabModulation.UseVisualStyleBackColor = true;
            // 
            // channelUserControl1
            // 
            this.channelUserControl1.Location = new System.Drawing.Point(6, 6);
            this.channelUserControl1.Name = "channelUserControl1";
            this.channelUserControl1.Size = new System.Drawing.Size(224, 88);
            this.channelUserControl1.TabIndex = 0;
            // 
            // channelUserControl2
            // 
            this.channelUserControl2.Location = new System.Drawing.Point(6, 6);
            this.channelUserControl2.Name = "channelUserControl2";
            this.channelUserControl2.Size = new System.Drawing.Size(224, 88);
            this.channelUserControl2.TabIndex = 1;
            // 
            // dualChannelUserControl1
            // 
            this.dualChannelUserControl1.Location = new System.Drawing.Point(6, 6);
            this.dualChannelUserControl1.Name = "dualChannelUserControl1";
            this.dualChannelUserControl1.Size = new System.Drawing.Size(241, 72);
            this.dualChannelUserControl1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Modulation Mode";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "FM";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Modulation Levels";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(159, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Channel";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(159, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "0";
            // 
            // MainGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 491);
            this.Controls.Add(this.tabControlMode);
            this.Controls.Add(this.fullResetButton);
            this.Controls.Add(this.deviceListBox);
            this.Name = "MainGUI";
            this.Text = "CQT DDS Control GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainGUI_FormClosing_1);
            this.channelTabControl.ResumeLayout(false);
            this.channelZeroTabPage.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabControlMode.ResumeLayout(false);
            this.tabSingletone.ResumeLayout(false);
            this.tabModulation.ResumeLayout(false);
            this.tabModulation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox deviceListBox;
        private System.Windows.Forms.Button fullResetButton;
        private System.Windows.Forms.TabControl channelTabControl;
        private System.Windows.Forms.TabPage channelZeroTabPage;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private ChannelUserControl channelUserControl1;
        private System.Windows.Forms.Button setChannelButton;
        private ChannelUserControl channelUserControl2;
        private DualChannelUserControl dualChannelUserControl1;
        private System.Windows.Forms.TabControl tabControlMode;
        private System.Windows.Forms.TabPage tabSingletone;
        private System.Windows.Forms.TabPage tabModulation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;

    }
}

