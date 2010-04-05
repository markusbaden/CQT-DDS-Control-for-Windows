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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.channelUserControl2 = new DDSControl.ChannelUserControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dualChannelUserControl1 = new DDSControl.DualChannelUserControl();
            this.setChannelButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSingleTone = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.channelTabControl.SuspendLayout();
            this.channelZeroTabPage.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageSingleTone.SuspendLayout();
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
            // channelUserControl1
            // 
            this.channelUserControl1.Location = new System.Drawing.Point(6, 6);
            this.channelUserControl1.Name = "channelUserControl1";
            this.channelUserControl1.Size = new System.Drawing.Size(224, 88);
            this.channelUserControl1.TabIndex = 0;
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
            // channelUserControl2
            // 
            this.channelUserControl2.Location = new System.Drawing.Point(6, 6);
            this.channelUserControl2.Name = "channelUserControl2";
            this.channelUserControl2.Size = new System.Drawing.Size(224, 88);
            this.channelUserControl2.TabIndex = 1;
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
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageSingleTone);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(386, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(388, 340);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPageSingleTone
            // 
            this.tabPageSingleTone.Controls.Add(this.channelTabControl);
            this.tabPageSingleTone.Controls.Add(this.setChannelButton);
            this.tabPageSingleTone.Location = new System.Drawing.Point(4, 22);
            this.tabPageSingleTone.Name = "tabPageSingleTone";
            this.tabPageSingleTone.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSingleTone.Size = new System.Drawing.Size(380, 314);
            this.tabPageSingleTone.TabIndex = 0;
            this.tabPageSingleTone.Text = "Singletone";
            this.tabPageSingleTone.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(192, 74);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // MainGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 491);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.fullResetButton);
            this.Controls.Add(this.deviceListBox);
            this.Name = "MainGUI";
            this.Text = "CQT DDS Control GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainGUI_FormClosing_1);
            this.channelTabControl.ResumeLayout(false);
            this.channelZeroTabPage.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageSingleTone.ResumeLayout(false);
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSingleTone;
        private System.Windows.Forms.TabPage tabPage4;

    }
}

