namespace Seagate.AAS.HGA.HST.UI
{
    partial class PanelAnalogInput
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
            this.labelAnalogInputName = new System.Windows.Forms.Label();
            this.labelAnalogInputValue = new System.Windows.Forms.Label();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // labelAnalogInputName
            // 
            this.labelAnalogInputName.Location = new System.Drawing.Point(5, 5);
            this.labelAnalogInputName.Name = "labelAnalogInputName";
            this.labelAnalogInputName.Size = new System.Drawing.Size(70, 18);
            this.labelAnalogInputName.TabIndex = 0;
            this.labelAnalogInputName.Text = "Name";
            this.labelAnalogInputName.VisibleChanged += new System.EventHandler(this.labelAnalogInputName_VisibleChanged);
            // 
            // labelAnalogInputValue
            // 
            this.labelAnalogInputValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelAnalogInputValue.Location = new System.Drawing.Point(81, 5);
            this.labelAnalogInputValue.Name = "labelAnalogInputValue";
            this.labelAnalogInputValue.Size = new System.Drawing.Size(80, 17);
            this.labelAnalogInputValue.TabIndex = 1;
            this.labelAnalogInputValue.Text = "Value";
            this.labelAnalogInputValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // PanelAnalogInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelAnalogInputValue);
            this.Controls.Add(this.labelAnalogInputName);
            this.Name = "PanelAnalogInput";
            this.Size = new System.Drawing.Size(167, 25);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label labelAnalogInputName;
        private System.Windows.Forms.Label labelAnalogInputValue;
        private System.Windows.Forms.Timer tmrUpdate;
    }
}
