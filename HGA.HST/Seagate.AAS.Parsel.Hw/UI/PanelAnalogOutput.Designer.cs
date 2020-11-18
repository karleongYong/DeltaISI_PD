namespace Seagate.AAS.Parsel.Hw
{
    partial class PanelAnalogOutput
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
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.numberAnalog = new Seagate.AAS.UI.FormattedNumber();
            this.labelUnit = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.numberLimitMin = new Seagate.AAS.UI.FormattedNumber();
            this.numberLimitMax = new Seagate.AAS.UI.FormattedNumber();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar
            // 
            this.trackBar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trackBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.trackBar.LargeChange = 25;
            this.trackBar.Location = new System.Drawing.Point(0, 21);
            this.trackBar.Maximum = 100;
            this.trackBar.Name = "trackBar";
            this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar.Size = new System.Drawing.Size(45, 84);
            this.trackBar.TabIndex = 1;
            this.trackBar.TickFrequency = 25;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // numberAnalog
            // 
            this.numberAnalog.AutoSize = true;
            this.numberAnalog.Location = new System.Drawing.Point(71, 53);
            this.numberAnalog.Name = "numberAnalog";
            this.numberAnalog.Number = 0;
            this.numberAnalog.NumberFormat = "0.000";
            this.numberAnalog.Size = new System.Drawing.Size(34, 13);
            this.numberAnalog.TabIndex = 2;
            this.numberAnalog.Text = "0.000";
            this.numberAnalog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelUnit
            // 
            this.labelUnit.AutoSize = true;
            this.labelUnit.Location = new System.Drawing.Point(111, 53);
            this.labelUnit.Name = "labelUnit";
            this.labelUnit.Size = new System.Drawing.Size(26, 13);
            this.labelUnit.TabIndex = 3;
            this.labelUnit.Text = "Unit";
            // 
            // labelName
            // 
            this.labelName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelName.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(0, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(141, 21);
            this.labelName.TabIndex = 4;
            this.labelName.Text = "Name";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // numberLimitMin
            // 
            this.numberLimitMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numberLimitMin.AutoSize = true;
            this.numberLimitMin.Location = new System.Drawing.Point(32, 86);
            this.numberLimitMin.Name = "numberLimitMin";
            this.numberLimitMin.Number = 0;
            this.numberLimitMin.NumberFormat = "0.000";
            this.numberLimitMin.Size = new System.Drawing.Size(34, 13);
            this.numberLimitMin.TabIndex = 5;
            this.numberLimitMin.Text = "0.000";
            this.numberLimitMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numberLimitMax
            // 
            this.numberLimitMax.AutoSize = true;
            this.numberLimitMax.Location = new System.Drawing.Point(32, 24);
            this.numberLimitMax.Name = "numberLimitMax";
            this.numberLimitMax.Number = 0;
            this.numberLimitMax.NumberFormat = "0.000";
            this.numberLimitMax.Size = new System.Drawing.Size(34, 13);
            this.numberLimitMax.TabIndex = 6;
            this.numberLimitMax.Text = "0.000";
            this.numberLimitMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PanelAnalogOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.numberLimitMax);
            this.Controls.Add(this.numberLimitMin);
            this.Controls.Add(this.labelUnit);
            this.Controls.Add(this.numberAnalog);
            this.Controls.Add(this.trackBar);
            this.Controls.Add(this.labelName);
            this.Name = "PanelAnalogOutput";
            this.Size = new System.Drawing.Size(141, 105);
            this.VisibleChanged += new System.EventHandler(this.PanelAnalogOutput_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar;
        private Seagate.AAS.UI.FormattedNumber numberAnalog;
        private System.Windows.Forms.Label labelUnit;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Timer timer1;
        private Seagate.AAS.UI.FormattedNumber numberLimitMin;
        private Seagate.AAS.UI.FormattedNumber numberLimitMax;

    }
}
