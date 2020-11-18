namespace Seagate.AAS.HGA.HST.UI
{
    partial class FormMainE95
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainE95));
            this.timerChkWindows = new System.Windows.Forms.Timer(this.components);
            this.panelCommand1 = new Seagate.AAS.HGA.HST.UI.PanelCommand(_workcell);
            this.Navigation1 = new Seagate.AAS.HGA.HST.UI.PanelNavigation();
            this.panelTitle = new Seagate.AAS.HGA.HST.UI.PanelTitle();
            this.testProbeComPort = new System.IO.Ports.SerialPort(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLblDiscIcon = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusProgressDisk = new XyratexOSC.UI.ToolStripProgressBarEnhanced();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // panelTitle
            // 
            this.panelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(102)))), ((int)(((byte)(153)))));            
            this.panelTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(1664, 69);
            this.panelTitle.TabIndex = 9;            
            // 
            // panelCommand1
            // 
            this.panelCommand1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCommand1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelCommand1.Location = new System.Drawing.Point(1589, 69);
            this.panelCommand1.Name = "panelCommand1";
            this.panelCommand1.Size = new System.Drawing.Size(75, 822);
            this.panelCommand1.TabIndex = 11;
            // 
            // timerChkWindows
            // 
            this.timerChkWindows.Interval = 500;
            this.timerChkWindows.Tick += new System.EventHandler(this.timerChkWindows_Tick);
            // 
            // Navigation1
            // 
            this.Navigation1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(102)))), ((int)(((byte)(153)))));
            this.Navigation1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Navigation1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Navigation1.Location = new System.Drawing.Point(0, 869);
            this.Navigation1.Name = "Navigation1";
            this.Navigation1.RunMode = false;
            this.Navigation1.Size = new System.Drawing.Size(1664, 71);
            this.Navigation1.TabIndex = 10;
            // 
            // testProbeComPort
            // 
            this.testProbeComPort.BaudRate = 19200;
            this.testProbeComPort.ReadBufferSize = 512;
            this.testProbeComPort.ReadTimeout = 6000;
            this.testProbeComPort.WriteBufferSize = 512;
            this.testProbeComPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.testProbeComPort_DataReceived);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLblDiscIcon,
            this.statusProgressDisk,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(20, 940);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1264, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLblDiscIcon
            //
            this.statusLblDiscIcon.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.statusLblDiscIcon.Image = global::Seagate.AAS.HGA.HST.Properties.Resources.harddisk_icon;
            this.statusLblDiscIcon.Name = "statusLblDiscIcon";
            this.statusLblDiscIcon.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statusLblDiscIcon.Size = new System.Drawing.Size(28, 20);      
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.ForeColor = System.Drawing.Color.LightSalmon;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(1061, 19);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolStripProgressBar1.Value = 100;
            this.toolStripProgressBar1.MarqueeAnimationSpeed = 20;
            this.toolStripProgressBar1.Visible = false;
            this.toolStripProgressBar1.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            // 
            // statusProgressDisk
            // 
            this.statusProgressDisk.BackColor = System.Drawing.Color.DodgerBlue;
            this.statusProgressDisk.BackColor2 = System.Drawing.Color.Empty;
            this.statusProgressDisk.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statusProgressDisk.Label = null;
            this.statusProgressDisk.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.statusProgressDisk.Maximum = 100;
            this.statusProgressDisk.Maximum2 = 0;
            this.statusProgressDisk.Minimum = 0;
            this.statusProgressDisk.Minimum2 = 0;
            this.statusProgressDisk.Name = "statusProgressDisk";
            this.statusProgressDisk.Size = new System.Drawing.Size(160, 19);
            this.statusProgressDisk.Value = 0;
            this.statusProgressDisk.Value2 = 0;
            // 
            // FormMainE95
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1664, 962);
            this.Controls.Add(this.panelTitle);  
            this.Controls.Add(this.panelCommand1);
            this.Controls.Add(this.Navigation1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1280, 1024);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FormMainE95";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMainE95_FormClosing);
           // this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        
        private PanelTitle panelTitle;
        public PanelNavigation Navigation1;
        private PanelCommand panelCommand1;
        private System.IO.Ports.SerialPort testProbeComPort;
        private System.Windows.Forms.Timer timerChkWindows;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLblDiscIcon;
        private XyratexOSC.UI.ToolStripProgressBarEnhanced statusProgressDisk;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
    }
}
