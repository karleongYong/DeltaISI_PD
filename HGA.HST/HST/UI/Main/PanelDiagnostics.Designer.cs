namespace Seagate.AAS.HGA.HST.UI.Main
{
    partial class PanelDiagnostics
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();            
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.rfidPanel1 = new Seagate.AAS.HGA.HST.UI.Diagnostics.RFIDPanel();
            this.MCPanel = new Seagate.AAS.HGA.HST.UI.Diagnostics.MotionControlPanel();
            this.IOPanel = new Seagate.AAS.HGA.HST.UI.Diagnostics.IOControlPanel();
            this.TEPanel = new Seagate.AAS.HGA.HST.UI.Diagnostics.TestElectronicsPanel();
            this.SCPanel = new Seagate.AAS.HGA.HST.UI.Diagnostics.SelfCheckPanel();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1167, 603);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.rfidPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPageRFID";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1159, 577);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "RFID";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // rfidPanel1
            // 
            this.rfidPanel1.Location = new System.Drawing.Point(6, 6);
            this.rfidPanel1.Name = "rfidPanel1";
            this.rfidPanel1.Size = new System.Drawing.Size(552, 565);
            this.rfidPanel1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.MCPanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPageMotionController";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1159, 577);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Motion Controller";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.IOPanel);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPageIOController";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1159, 577);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "IO Controller";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.TEPanel);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1159, 577);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "Test Electronics";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            //
            this.tabPage5.Controls.Add(this.SCPanel);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1159, 577);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "Communication Self Check";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // PanelDiagnostics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "PanelDiagnostics";
            this.Size = new System.Drawing.Size(1167, 603);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private Diagnostics.RFIDPanel rfidPanel1;
        private Diagnostics.MotionControlPanel MCPanel;
        private Diagnostics.IOControlPanel IOPanel;
        private Diagnostics.TestElectronicsPanel TEPanel;
        public Diagnostics.SelfCheckPanel SCPanel;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
    }
}
