namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    partial class PanelDigitalOutput
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
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.ledOutput = new Seagate.AAS.UI.Led();
            this.SuspendLayout();
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // ledOutput
            // 
            this.ledOutput.DisplayAsButton = true;
            this.ledOutput.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledOutput.Location = new System.Drawing.Point(0, 0);
            this.ledOutput.Name = "ledOutput";
            this.ledOutput.Size = new System.Drawing.Size(250, 23);
            this.ledOutput.State = false;
            this.ledOutput.TabIndex = 6;
            this.ledOutput.Text = "Output";
            this.ledOutput.VisibleChanged += new System.EventHandler(this.ledOutput_VisibleChanged);
            this.ledOutput.Click += new System.EventHandler(this.ledOutput_Click);
            // 
            // PanelOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ledOutput);
            this.Name = "PanelOutput";
            this.Size = new System.Drawing.Size(250, 23);
            this.ResumeLayout(false);

        }

        #endregion

        public AAS.UI.Led ledOutput;
        private System.Windows.Forms.Timer tmrUpdate;
    }
}
