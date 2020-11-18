namespace Seagate.AAS.Parsel.Device.SeaveyorZone
{
    partial class PanelSeaveyorZone
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelSeaveyorZone));
            this.ledInPosition = new Seagate.AAS.UI.Led();
            this.ledZoneSensor = new Seagate.AAS.UI.Led();
            this.indicatorButtonInhibit = new Seagate.AAS.UI.IndicatorButton();
            this.indicatorButtonReverse = new Seagate.AAS.UI.IndicatorButton();
            this.labelZoneName = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ledInPosition
            // 
            this.ledInPosition.DisplayAsButton = false;
            this.ledInPosition.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledInPosition.Location = new System.Drawing.Point(1, 36);
            this.ledInPosition.Name = "ledInPosition";
            this.ledInPosition.Size = new System.Drawing.Size(92, 26);
            this.ledInPosition.State = false;
            this.ledInPosition.TabIndex = 0;
            this.ledInPosition.Text = "InPosition";
            // 
            // ledZoneSensor
            // 
            this.ledZoneSensor.DisplayAsButton = false;
            this.ledZoneSensor.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledZoneSensor.Location = new System.Drawing.Point(1, 68);
            this.ledZoneSensor.Name = "ledZoneSensor";
            this.ledZoneSensor.Size = new System.Drawing.Size(92, 26);
            this.ledZoneSensor.State = false;
            this.ledZoneSensor.TabIndex = 1;
            this.ledZoneSensor.Text = "Zone Sensor";
            // 
            // indicatorButtonInhibit
            // 
            this.indicatorButtonInhibit.Image = ((System.Drawing.Image)(resources.GetObject("indicatorButtonInhibit.Image")));
            this.indicatorButtonInhibit.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.indicatorButtonInhibit.Location = new System.Drawing.Point(3, 100);
            this.indicatorButtonInhibit.Name = "indicatorButtonInhibit";
            this.indicatorButtonInhibit.Size = new System.Drawing.Size(95, 38);
            this.indicatorButtonInhibit.State = false;
            this.indicatorButtonInhibit.TabIndex = 2;
            this.indicatorButtonInhibit.Text = "Inhibit";
            this.indicatorButtonInhibit.UseVisualStyleBackColor = true;
            this.indicatorButtonInhibit.Click += new System.EventHandler(this.indicatorButtonInhibit_Click_1);
            // 
            // indicatorButtonReverse
            // 
            this.indicatorButtonReverse.Image = ((System.Drawing.Image)(resources.GetObject("indicatorButtonReverse.Image")));
            this.indicatorButtonReverse.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.indicatorButtonReverse.Location = new System.Drawing.Point(0, 163);
            this.indicatorButtonReverse.Name = "indicatorButtonReverse";
            this.indicatorButtonReverse.Size = new System.Drawing.Size(95, 37);
            this.indicatorButtonReverse.State = false;
            this.indicatorButtonReverse.TabIndex = 3;
            this.indicatorButtonReverse.Text = "Reverse";
            this.indicatorButtonReverse.UseVisualStyleBackColor = true;
            this.indicatorButtonReverse.Visible = false;
            //

            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);


            // 
            // labelZoneName
            // 
            this.labelZoneName.AutoSize = true;
            this.labelZoneName.Location = new System.Drawing.Point(18, 12);
            this.labelZoneName.Name = "labelZoneName";
            this.labelZoneName.Size = new System.Drawing.Size(63, 13);
            this.labelZoneName.TabIndex = 4;
            this.labelZoneName.Text = "Zone Name";
            // 
            // PanelSeaveyorZone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelZoneName);
            this.Controls.Add(this.indicatorButtonReverse);
            this.Controls.Add(this.indicatorButtonInhibit);
            this.Controls.Add(this.ledZoneSensor);
            this.Controls.Add(this.ledInPosition);
            this.Name = "PanelSeaveyorZone";
            this.VisibleChanged += new System.EventHandler(this.Panel_VisibleChanged);
            this.Size = new System.Drawing.Size(100, 142);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UI.Led ledInPosition;
        private UI.Led ledZoneSensor;
        private UI.IndicatorButton indicatorButtonInhibit;
        private UI.IndicatorButton indicatorButtonReverse;
        private System.Windows.Forms.Label labelZoneName;
        private System.Windows.Forms.Timer timer1;
    }
}
