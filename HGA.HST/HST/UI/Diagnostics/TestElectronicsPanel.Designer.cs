namespace Seagate.AAS.HGA.HST.UI.Diagnostics
{
    partial class TestElectronicsPanel
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
            this.groupBoxTemperatures = new System.Windows.Forms.GroupBox();
            this.textBoxExhaustTemperature = new System.Windows.Forms.TextBox();
            this.textBoxWorkingZoneTemperature = new System.Windows.Forms.TextBox();
            this.textBoxMeasurementBoardTemperature = new System.Windows.Forms.TextBox();
            this.labelExhaustTemparature = new System.Windows.Forms.Label();
            this.labelWorkingZoneTemparature = new System.Windows.Forms.Label();
            this.labelMeasurementBoardTemparature = new System.Windows.Forms.Label();
            this.buttonGetTemperatures = new System.Windows.Forms.Button();
            this.groupBoxTemperatures.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxTemperatures
            // 
            this.groupBoxTemperatures.Controls.Add(this.textBoxExhaustTemperature);
            this.groupBoxTemperatures.Controls.Add(this.textBoxWorkingZoneTemperature);
            this.groupBoxTemperatures.Controls.Add(this.textBoxMeasurementBoardTemperature);
            this.groupBoxTemperatures.Controls.Add(this.labelExhaustTemparature);
            this.groupBoxTemperatures.Controls.Add(this.labelWorkingZoneTemparature);
            this.groupBoxTemperatures.Controls.Add(this.labelMeasurementBoardTemparature);
            this.groupBoxTemperatures.Controls.Add(this.buttonGetTemperatures);
            this.groupBoxTemperatures.Location = new System.Drawing.Point(29, 48);
            this.groupBoxTemperatures.Name = "groupBoxTemperatures";
            this.groupBoxTemperatures.Size = new System.Drawing.Size(558, 212);
            this.groupBoxTemperatures.TabIndex = 0;
            this.groupBoxTemperatures.TabStop = false;
            this.groupBoxTemperatures.Text = "Temperatures";
            // 
            // textBoxExhaustTemperature
            // 
            this.textBoxExhaustTemperature.Location = new System.Drawing.Point(281, 89);
            this.textBoxExhaustTemperature.Name = "textBoxExhaustTemperature";
            this.textBoxExhaustTemperature.ReadOnly = true;
            this.textBoxExhaustTemperature.Size = new System.Drawing.Size(86, 20);
            this.textBoxExhaustTemperature.TabIndex = 6;
            this.textBoxExhaustTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxWorkingZoneTemperature
            // 
            this.textBoxWorkingZoneTemperature.Location = new System.Drawing.Point(157, 89);
            this.textBoxWorkingZoneTemperature.Name = "textBoxWorkingZoneTemperature";
            this.textBoxWorkingZoneTemperature.ReadOnly = true;
            this.textBoxWorkingZoneTemperature.Size = new System.Drawing.Size(95, 20);
            this.textBoxWorkingZoneTemperature.TabIndex = 5;
            this.textBoxWorkingZoneTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxMeasurementBoardTemperature
            // 
            this.textBoxMeasurementBoardTemperature.Location = new System.Drawing.Point(32, 89);
            this.textBoxMeasurementBoardTemperature.Name = "textBoxMeasurementBoardTemperature";
            this.textBoxMeasurementBoardTemperature.ReadOnly = true;
            this.textBoxMeasurementBoardTemperature.Size = new System.Drawing.Size(99, 20);
            this.textBoxMeasurementBoardTemperature.TabIndex = 4;
            this.textBoxMeasurementBoardTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelExhaustTemparature
            // 
            this.labelExhaustTemparature.AutoSize = true;
            this.labelExhaustTemparature.Location = new System.Drawing.Point(278, 52);
            this.labelExhaustTemparature.Name = "labelExhaustTemparature";
            this.labelExhaustTemparature.Size = new System.Drawing.Size(45, 13);
            this.labelExhaustTemparature.TabIndex = 3;
            this.labelExhaustTemparature.Text = "Exhaust";
            // 
            // labelWorkingZoneTemparature
            // 
            this.labelWorkingZoneTemparature.AutoSize = true;
            this.labelWorkingZoneTemparature.Location = new System.Drawing.Point(154, 52);
            this.labelWorkingZoneTemparature.Name = "labelWorkingZoneTemparature";
            this.labelWorkingZoneTemparature.Size = new System.Drawing.Size(75, 13);
            this.labelWorkingZoneTemparature.TabIndex = 2;
            this.labelWorkingZoneTemparature.Text = "Working Zone";
            // 
            // labelMeasurementBoardTemparature
            // 
            this.labelMeasurementBoardTemparature.AutoSize = true;
            this.labelMeasurementBoardTemparature.Location = new System.Drawing.Point(29, 51);
            this.labelMeasurementBoardTemparature.Name = "labelMeasurementBoardTemparature";
            this.labelMeasurementBoardTemparature.Size = new System.Drawing.Size(102, 13);
            this.labelMeasurementBoardTemparature.TabIndex = 1;
            this.labelMeasurementBoardTemparature.Text = "Measurement Board";
            // 
            // buttonGetTemperatures
            // 
            this.buttonGetTemperatures.Location = new System.Drawing.Point(405, 80);
            this.buttonGetTemperatures.Name = "buttonGetTemperatures";
            this.buttonGetTemperatures.Size = new System.Drawing.Size(107, 37);
            this.buttonGetTemperatures.TabIndex = 0;
            this.buttonGetTemperatures.Text = "Get Temperatures";
            this.buttonGetTemperatures.UseVisualStyleBackColor = true;
            this.buttonGetTemperatures.Click += new System.EventHandler(this.buttonGetTemperatures_Click);
            // 
            // TestElectronicsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxTemperatures);
            this.Name = "TestElectronicsPanel";
            this.Size = new System.Drawing.Size(677, 556);
            this.Enter += new System.EventHandler(this.TestElectronicsPanel_Enter);
            this.groupBoxTemperatures.ResumeLayout(false);
            this.groupBoxTemperatures.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxTemperatures;
        private System.Windows.Forms.TextBox textBoxExhaustTemperature;
        private System.Windows.Forms.TextBox textBoxWorkingZoneTemperature;
        private System.Windows.Forms.TextBox textBoxMeasurementBoardTemperature;
        private System.Windows.Forms.Label labelExhaustTemparature;
        private System.Windows.Forms.Label labelWorkingZoneTemparature;
        private System.Windows.Forms.Label labelMeasurementBoardTemparature;
        private System.Windows.Forms.Button buttonGetTemperatures;
    }
}
