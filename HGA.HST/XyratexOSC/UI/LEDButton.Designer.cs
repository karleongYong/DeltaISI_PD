namespace XyratexOSC.UI
{
    partial class LEDButton
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
            this.SuspendLayout();
            // 
            // LEDButton
            // 
            this.Appearance = System.Windows.Forms.Appearance.Button;
            this.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.FlatAppearance.BorderSize = 0;
            this.FlatAppearance.CheckedBackColor = System.Drawing.SystemColors.Control;
            this.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Control;
            this.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Image = global::XyratexOSC.Properties.Resources.LEDBTN_Grey_up;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Size = new System.Drawing.Size(16, 16);
            this.CheckedChanged += new System.EventHandler(this.LEDButton_CheckedChanged);
            this.MouseCaptureChanged += new System.EventHandler(this.LEDButton_MouseCaptureChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LEDButton_MouseDown);
            this.MouseLeave += new System.EventHandler(this.LEDButton_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LEDButton_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LEDButton_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
