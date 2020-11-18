namespace Seagate.AAS.HGA.HST.UI
{
    partial class PanelCommand
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelCommand));
            this.panel = new System.Windows.Forms.Panel();
            this.buttonSystemInitial = new System.Windows.Forms.Button();
            this.btnOffAlarm = new System.Windows.Forms.Button();
            this.btnSaveConfigToProcessor = new System.Windows.Forms.Button();
            this.lblDoorStatus = new System.Windows.Forms.Label();
            this.lblRedLight = new System.Windows.Forms.Label();
            this.lblTowerLight = new System.Windows.Forms.Label();
            this.lblAmberLight = new System.Windows.Forms.Label();
            this.btnRunTestScript = new System.Windows.Forms.Button();
            this.btnWindows = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonRun = new System.Windows.Forms.ToolStripButton();
            this.buttonStop = new System.Windows.Forms.ToolStripButton();
            this.buttonPause = new System.Windows.Forms.ToolStripButton();
            this.buttonPurge = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.Controls.Add(this.buttonSystemInitial);
            this.panel.Controls.Add(this.btnOffAlarm);
            this.panel.Controls.Add(this.btnSaveConfigToProcessor);
            this.panel.Controls.Add(this.lblDoorStatus);
            this.panel.Controls.Add(this.lblRedLight);
            this.panel.Controls.Add(this.lblTowerLight);
            this.panel.Controls.Add(this.lblAmberLight);
            this.panel.Controls.Add(this.btnRunTestScript);
            this.panel.Controls.Add(this.btnWindows);
            this.panel.Controls.Add(this.toolStrip1);
            this.panel.Name = "panel";
            // 
            // buttonSystemInitial
            // 
            this.buttonSystemInitial.BackgroundImage = global::Seagate.AAS.HGA.HST.Properties.Resources.Init;
            resources.ApplyResources(this.buttonSystemInitial, "buttonSystemInitial");
            this.buttonSystemInitial.Name = "buttonSystemInitial";
            this.buttonSystemInitial.UseVisualStyleBackColor = true;
            this.buttonSystemInitial.Click += new System.EventHandler(this.buttonSystemInitial_Click);
            // 
            // btnOffAlarm
            // 
            resources.ApplyResources(this.btnOffAlarm, "btnOffAlarm");
            this.btnOffAlarm.Name = "btnOffAlarm";
            this.btnOffAlarm.UseVisualStyleBackColor = true;
            this.btnOffAlarm.Click += new System.EventHandler(this.btnOffAlarm_Click);
            // 
            // btnSaveConfigToProcessor
            // 
            resources.ApplyResources(this.btnSaveConfigToProcessor, "btnSaveConfigToProcessor");
            this.btnSaveConfigToProcessor.Name = "btnSaveConfigToProcessor";
            this.btnSaveConfigToProcessor.UseVisualStyleBackColor = true;
            this.btnSaveConfigToProcessor.Click += new System.EventHandler(this.btnSaveConfigToProcessor_Click);
            // 
            // lblDoorStatus
            // 
            this.lblDoorStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblDoorStatus, "lblDoorStatus");
            this.lblDoorStatus.Name = "lblDoorStatus";
            this.toolTip1.SetToolTip(this.lblDoorStatus, resources.GetString("lblDoorStatus.ToolTip"));
            // 
            // lblRedLight
            // 
            this.lblRedLight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblRedLight, "lblRedLight");
            this.lblRedLight.Name = "lblRedLight";
            this.toolTip1.SetToolTip(this.lblRedLight, resources.GetString("lblRedLight.ToolTip"));
            // 
            // lblTowerLight
            // 
            resources.ApplyResources(this.lblTowerLight, "lblTowerLight");
            this.lblTowerLight.Name = "lblTowerLight";
            // 
            // lblAmberLight
            // 
            this.lblAmberLight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblAmberLight, "lblAmberLight");
            this.lblAmberLight.Name = "lblAmberLight";
            this.toolTip1.SetToolTip(this.lblAmberLight, resources.GetString("lblAmberLight.ToolTip"));
            // 
            // btnRunTestScript
            // 
            resources.ApplyResources(this.btnRunTestScript, "btnRunTestScript");
            this.btnRunTestScript.Name = "btnRunTestScript";
            this.btnRunTestScript.UseVisualStyleBackColor = true;
            this.btnRunTestScript.Click += new System.EventHandler(this.btnRunTestScript_Click);
            // 
            // btnWindows
            // 
            this.btnWindows.BackgroundImage = global::Seagate.AAS.HGA.HST.Properties.Resources.windows;
            resources.ApplyResources(this.btnWindows, "btnWindows");
            this.btnWindows.Name = "btnWindows";
            this.btnWindows.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonRun,
            this.buttonStop,
            this.buttonPause,
            this.buttonPurge,
            this.toolStripSeparator1});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Name = "toolStrip1";
            // 
            // buttonRun
            // 
            this.buttonRun.Image = global::Seagate.AAS.HGA.HST.Properties.Resources.play_64x64;
            resources.ApplyResources(this.buttonRun, "buttonRun");
            this.buttonRun.Margin = new System.Windows.Forms.Padding(0, 6, 0, 8);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Image = global::Seagate.AAS.HGA.HST.Properties.Resources.stop_64x64;
            resources.ApplyResources(this.buttonStop, "buttonStop");
            this.buttonStop.Margin = new System.Windows.Forms.Padding(0, 6, 0, 8);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Image = global::Seagate.AAS.HGA.HST.Properties.Resources.pause_64x64;
            resources.ApplyResources(this.buttonPause, "buttonPause");
            this.buttonPause.Margin = new System.Windows.Forms.Padding(0, 6, 0, 8);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonPurge
            // 
            resources.ApplyResources(this.buttonPurge, "buttonPurge");
            this.buttonPurge.Margin = new System.Windows.Forms.Padding(0, 6, 0, 8);
            this.buttonPurge.Name = "buttonPurge";
            this.buttonPurge.Click += new System.EventHandler(this.buttonPurge_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // PanelCommand
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panel);
            this.Name = "PanelCommand";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ToolStripButton buttonRun;
        private System.Windows.Forms.ToolStripButton buttonStop;
        private System.Windows.Forms.ToolStripButton buttonPause;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonPurge;
        private System.Windows.Forms.Button btnWindows;
        private System.Windows.Forms.Button btnRunTestScript;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnOffAlarm;
        private System.Windows.Forms.Label lblDoorStatus;
        private System.Windows.Forms.Label lblRedLight;
        private System.Windows.Forms.Label lblTowerLight;
        private System.Windows.Forms.Label lblAmberLight;
        private System.Windows.Forms.Button btnSaveConfigToProcessor;
        private System.Windows.Forms.Button buttonSystemInitial;
    }
}
