namespace Seagate.AAS.HGA.HST.UI
{
    partial class PanelTitle
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelTitle));
            this.lblCurrentView = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelFirmware = new System.Windows.Forms.Label();
            this.panelConfig = new System.Windows.Forms.Panel();
            this.headerUserAccess1 = new Seagate.AAS.HGA.HST.UI.HeaderUserAccess();
            this.label1 = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.RunModeLbl = new System.Windows.Forms.Label();
            this.label100 = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.panelDivider = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel2.SuspendLayout();
            this.panelConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCurrentView
            // 
            this.lblCurrentView.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblCurrentView, "lblCurrentView");
            this.lblCurrentView.ForeColor = System.Drawing.Color.White;
            this.lblCurrentView.Name = "lblCurrentView";
            // 
            // labelVersion
            // 
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelVersion, "labelVersion");
            this.labelVersion.ForeColor = System.Drawing.Color.White;
            this.labelVersion.Name = "labelVersion";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.labelFirmware);
            this.panel2.Controls.Add(this.panelConfig);
            this.panel2.Controls.Add(this.lblCurrentView);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Name = "label2";
            // 
            // labelFirmware
            // 
            this.labelFirmware.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelFirmware, "labelFirmware");
            this.labelFirmware.ForeColor = System.Drawing.Color.Yellow;
            this.labelFirmware.Name = "labelFirmware";
            // 
            // panelConfig
            // 
            this.panelConfig.Controls.Add(this.headerUserAccess1);
            this.panelConfig.Controls.Add(this.label1);
            this.panelConfig.Controls.Add(this.lblState);
            this.panelConfig.Controls.Add(this.RunModeLbl);
            this.panelConfig.Controls.Add(this.label100);
            this.panelConfig.Controls.Add(this.lblMessage);
            this.panelConfig.Controls.Add(this.labelVersion);
            resources.ApplyResources(this.panelConfig, "panelConfig");
            this.panelConfig.Name = "panelConfig";
            // 
            // headerUserAccess1
            // 
            resources.ApplyResources(this.headerUserAccess1, "headerUserAccess1");
            this.headerUserAccess1.BackColor = System.Drawing.Color.Transparent;
            this.headerUserAccess1.Name = "headerUserAccess1";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // lblState
            // 
            this.lblState.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblState, "lblState");
            this.lblState.ForeColor = System.Drawing.Color.White;
            this.lblState.Name = "lblState";
            // 
            // RunModeLbl
            // 
            this.RunModeLbl.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.RunModeLbl, "RunModeLbl");
            this.RunModeLbl.ForeColor = System.Drawing.Color.White;
            this.RunModeLbl.Name = "RunModeLbl";
            // 
            // label100
            // 
            this.label100.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label100, "label100");
            this.label100.ForeColor = System.Drawing.Color.White;
            this.label100.Name = "label100";
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblMessage, "lblMessage");
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Name = "lblMessage";
            // 
            // panelDivider
            // 
            this.panelDivider.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.panelDivider, "panelDivider");
            this.panelDivider.Name = "panelDivider";
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PanelTitle
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(102)))), ((int)(((byte)(153)))));
            this.Controls.Add(this.panelDivider);
            this.Controls.Add(this.panel2);
            this.Name = "PanelTitle";
            this.Load += new System.EventHandler(this.PanelTitle_Load);
            this.panel2.ResumeLayout(false);
            this.panelConfig.ResumeLayout(false);
            this.panelConfig.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblCurrentView;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelConfig;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label label100;
        private System.Windows.Forms.Panel panelDivider;
        public System.Windows.Forms.Label RunModeLbl;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private HeaderUserAccess headerUserAccess1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelFirmware;
    }
}
