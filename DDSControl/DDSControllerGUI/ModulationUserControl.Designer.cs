namespace DDSControl
{
    partial class ModulationUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxModulationMode = new System.Windows.Forms.ComboBox();
            this.labelChannelSelection = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelChannelWords = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxModulationMode
            // 
            this.comboBoxModulationMode.FormattingEnabled = true;
            this.comboBoxModulationMode.Items.AddRange(new object[] {
            "fm",
            "pm"});
            this.comboBoxModulationMode.Location = new System.Drawing.Point(119, 8);
            this.comboBoxModulationMode.Name = "comboBoxModulationMode";
            this.comboBoxModulationMode.Size = new System.Drawing.Size(121, 21);
            this.comboBoxModulationMode.TabIndex = 13;
            this.comboBoxModulationMode.Text = "fm";
            // 
            // labelChannelSelection
            // 
            this.labelChannelSelection.AutoSize = true;
            this.labelChannelSelection.Location = new System.Drawing.Point(116, 76);
            this.labelChannelSelection.Name = "labelChannelSelection";
            this.labelChannelSelection.Size = new System.Drawing.Size(13, 13);
            this.labelChannelSelection.TabIndex = 12;
            this.labelChannelSelection.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-63, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Channel";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(116, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Modulation Levels";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Modulation Mode";
            // 
            // labelChannelWords
            // 
            this.labelChannelWords.AutoSize = true;
            this.labelChannelWords.Location = new System.Drawing.Point(3, 107);
            this.labelChannelWords.Name = "labelChannelWords";
            this.labelChannelWords.Size = new System.Drawing.Size(80, 13);
            this.labelChannelWords.TabIndex = 15;
            this.labelChannelWords.Text = "Channel Words";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Channel";
            // 
            // ModulationUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelChannelWords);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxModulationMode);
            this.Controls.Add(this.labelChannelSelection);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "ModulationUserControl";
            this.Size = new System.Drawing.Size(243, 239);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxModulationMode;
        private System.Windows.Forms.Label labelChannelSelection;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelChannelWords;
        private System.Windows.Forms.Label label2;
    }
}
