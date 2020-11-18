namespace Seagate.AAS.HGA.HST.UI.Diagnostics
{
    partial class SelfCheckPanel
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
            this.groupBoxCamera = new System.Windows.Forms.GroupBox();
            this.textBoxOutputCameraStatus = new System.Windows.Forms.TextBox();
            this.textBoxInputCameraStatus = new System.Windows.Forms.TextBox();
            this.buttonCheckOutputCamera = new System.Windows.Forms.Button();
            this.buttonCheckInputCamera = new System.Windows.Forms.Button();
            this.labelOutputCamera = new System.Windows.Forms.Label();
            this.labelInputCamera = new System.Windows.Forms.Label();
            this.groupBoxTestElectronics = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFirmwareVersion = new System.Windows.Forms.TextBox();
            this.checkFirmwareVersion = new System.Windows.Forms.Button();
            this.textBoxLCRMeterStatus = new System.Windows.Forms.TextBox();
            this.textBoxElectronicsBoardStatus = new System.Windows.Forms.TextBox();
            this.buttonCheckLCRMeter = new System.Windows.Forms.Button();
            this.buttonCheckElectronicsBoard = new System.Windows.Forms.Button();
            this.labelLCRMeter = new System.Windows.Forms.Label();
            this.labelElectronicsBoard = new System.Windows.Forms.Label();
            this.groupBoxCamera.SuspendLayout();
            this.groupBoxTestElectronics.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCamera
            // 
            this.groupBoxCamera.Controls.Add(this.textBoxOutputCameraStatus);
            this.groupBoxCamera.Controls.Add(this.textBoxInputCameraStatus);
            this.groupBoxCamera.Controls.Add(this.buttonCheckOutputCamera);
            this.groupBoxCamera.Controls.Add(this.buttonCheckInputCamera);
            this.groupBoxCamera.Controls.Add(this.labelOutputCamera);
            this.groupBoxCamera.Controls.Add(this.labelInputCamera);
            this.groupBoxCamera.Location = new System.Drawing.Point(43, 59);
            this.groupBoxCamera.Name = "groupBoxCamera";
            this.groupBoxCamera.Size = new System.Drawing.Size(545, 118);
            this.groupBoxCamera.TabIndex = 0;
            this.groupBoxCamera.TabStop = false;
            this.groupBoxCamera.Text = "Camera";
            // 
            // textBoxOutputCameraStatus
            // 
            this.textBoxOutputCameraStatus.Location = new System.Drawing.Point(277, 69);
            this.textBoxOutputCameraStatus.Name = "textBoxOutputCameraStatus";
            this.textBoxOutputCameraStatus.ReadOnly = true;
            this.textBoxOutputCameraStatus.Size = new System.Drawing.Size(242, 20);
            this.textBoxOutputCameraStatus.TabIndex = 5;
            // 
            // textBoxInputCameraStatus
            // 
            this.textBoxInputCameraStatus.Location = new System.Drawing.Point(277, 40);
            this.textBoxInputCameraStatus.Name = "textBoxInputCameraStatus";
            this.textBoxInputCameraStatus.ReadOnly = true;
            this.textBoxInputCameraStatus.Size = new System.Drawing.Size(242, 20);
            this.textBoxInputCameraStatus.TabIndex = 4;
            // 
            // buttonCheckOutputCamera
            // 
            this.buttonCheckOutputCamera.Location = new System.Drawing.Point(187, 67);
            this.buttonCheckOutputCamera.Name = "buttonCheckOutputCamera";
            this.buttonCheckOutputCamera.Size = new System.Drawing.Size(75, 23);
            this.buttonCheckOutputCamera.TabIndex = 3;
            this.buttonCheckOutputCamera.Text = "Check";
            this.buttonCheckOutputCamera.UseVisualStyleBackColor = true;
            this.buttonCheckOutputCamera.Click += new System.EventHandler(this.buttonCheckOutputCamera_Click);
            // 
            // buttonCheckInputCamera
            // 
            this.buttonCheckInputCamera.Location = new System.Drawing.Point(187, 37);
            this.buttonCheckInputCamera.Name = "buttonCheckInputCamera";
            this.buttonCheckInputCamera.Size = new System.Drawing.Size(75, 23);
            this.buttonCheckInputCamera.TabIndex = 2;
            this.buttonCheckInputCamera.Text = "Check";
            this.buttonCheckInputCamera.UseVisualStyleBackColor = true;
            this.buttonCheckInputCamera.Click += new System.EventHandler(this.buttonCheckInputCamera_Click);
            // 
            // labelOutputCamera
            // 
            this.labelOutputCamera.AutoSize = true;
            this.labelOutputCamera.Location = new System.Drawing.Point(16, 67);
            this.labelOutputCamera.Name = "labelOutputCamera";
            this.labelOutputCamera.Size = new System.Drawing.Size(78, 13);
            this.labelOutputCamera.TabIndex = 1;
            this.labelOutputCamera.Text = "Output Camera";
            // 
            // labelInputCamera
            // 
            this.labelInputCamera.AutoSize = true;
            this.labelInputCamera.Location = new System.Drawing.Point(16, 37);
            this.labelInputCamera.Name = "labelInputCamera";
            this.labelInputCamera.Size = new System.Drawing.Size(70, 13);
            this.labelInputCamera.TabIndex = 0;
            this.labelInputCamera.Text = "Input Camera";
            // 
            // groupBoxTestElectronics
            // 
            this.groupBoxTestElectronics.Controls.Add(this.label1);
            this.groupBoxTestElectronics.Controls.Add(this.textBoxFirmwareVersion);
            this.groupBoxTestElectronics.Controls.Add(this.checkFirmwareVersion);
            this.groupBoxTestElectronics.Controls.Add(this.textBoxLCRMeterStatus);
            this.groupBoxTestElectronics.Controls.Add(this.textBoxElectronicsBoardStatus);
            this.groupBoxTestElectronics.Controls.Add(this.buttonCheckLCRMeter);
            this.groupBoxTestElectronics.Controls.Add(this.buttonCheckElectronicsBoard);
            this.groupBoxTestElectronics.Controls.Add(this.labelLCRMeter);
            this.groupBoxTestElectronics.Controls.Add(this.labelElectronicsBoard);
            this.groupBoxTestElectronics.Location = new System.Drawing.Point(43, 208);
            this.groupBoxTestElectronics.Name = "groupBoxTestElectronics";
            this.groupBoxTestElectronics.Size = new System.Drawing.Size(545, 156);
            this.groupBoxTestElectronics.TabIndex = 1;
            this.groupBoxTestElectronics.TabStop = false;
            this.groupBoxTestElectronics.Text = "Test Electronics";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Firmware Versioni";
            // 
            // textBoxFirmwareVersion
            // 
            this.textBoxFirmwareVersion.Location = new System.Drawing.Point(277, 115);
            this.textBoxFirmwareVersion.Name = "textBoxFirmwareVersion";
            this.textBoxFirmwareVersion.ReadOnly = true;
            this.textBoxFirmwareVersion.Size = new System.Drawing.Size(156, 20);
            this.textBoxFirmwareVersion.TabIndex = 7;
            // 
            // checkFirmwareVersion
            // 
            this.checkFirmwareVersion.Location = new System.Drawing.Point(187, 113);
            this.checkFirmwareVersion.Name = "checkFirmwareVersion";
            this.checkFirmwareVersion.Size = new System.Drawing.Size(75, 23);
            this.checkFirmwareVersion.TabIndex = 6;
            this.checkFirmwareVersion.Text = "Check";
            this.checkFirmwareVersion.UseVisualStyleBackColor = true;
            this.checkFirmwareVersion.Click += new System.EventHandler(this.checkFirmwareVersion_Click);
            // 
            // textBoxLCRMeterStatus
            // 
            this.textBoxLCRMeterStatus.Location = new System.Drawing.Point(277, 77);
            this.textBoxLCRMeterStatus.Name = "textBoxLCRMeterStatus";
            this.textBoxLCRMeterStatus.ReadOnly = true;
            this.textBoxLCRMeterStatus.Size = new System.Drawing.Size(156, 20);
            this.textBoxLCRMeterStatus.TabIndex = 5;
            // 
            // textBoxElectronicsBoardStatus
            // 
            this.textBoxElectronicsBoardStatus.Location = new System.Drawing.Point(277, 42);
            this.textBoxElectronicsBoardStatus.Name = "textBoxElectronicsBoardStatus";
            this.textBoxElectronicsBoardStatus.ReadOnly = true;
            this.textBoxElectronicsBoardStatus.Size = new System.Drawing.Size(156, 20);
            this.textBoxElectronicsBoardStatus.TabIndex = 4;
            // 
            // buttonCheckLCRMeter
            // 
            this.buttonCheckLCRMeter.Location = new System.Drawing.Point(187, 75);
            this.buttonCheckLCRMeter.Name = "buttonCheckLCRMeter";
            this.buttonCheckLCRMeter.Size = new System.Drawing.Size(75, 23);
            this.buttonCheckLCRMeter.TabIndex = 3;
            this.buttonCheckLCRMeter.Text = "Check";
            this.buttonCheckLCRMeter.UseVisualStyleBackColor = true;
            this.buttonCheckLCRMeter.Click += new System.EventHandler(this.buttonCheckLCRMeter_Click);
            // 
            // buttonCheckElectronicsBoard
            // 
            this.buttonCheckElectronicsBoard.Location = new System.Drawing.Point(187, 40);
            this.buttonCheckElectronicsBoard.Name = "buttonCheckElectronicsBoard";
            this.buttonCheckElectronicsBoard.Size = new System.Drawing.Size(75, 23);
            this.buttonCheckElectronicsBoard.TabIndex = 2;
            this.buttonCheckElectronicsBoard.Text = "Check";
            this.buttonCheckElectronicsBoard.UseVisualStyleBackColor = true;
            this.buttonCheckElectronicsBoard.Click += new System.EventHandler(this.buttonCheckElectronicsBoard_Click);
            // 
            // labelLCRMeter
            // 
            this.labelLCRMeter.AutoSize = true;
            this.labelLCRMeter.Location = new System.Drawing.Point(16, 75);
            this.labelLCRMeter.Name = "labelLCRMeter";
            this.labelLCRMeter.Size = new System.Drawing.Size(58, 13);
            this.labelLCRMeter.TabIndex = 1;
            this.labelLCRMeter.Text = "LCR Meter";
            // 
            // labelElectronicsBoard
            // 
            this.labelElectronicsBoard.AutoSize = true;
            this.labelElectronicsBoard.Location = new System.Drawing.Point(16, 40);
            this.labelElectronicsBoard.Name = "labelElectronicsBoard";
            this.labelElectronicsBoard.Size = new System.Drawing.Size(90, 13);
            this.labelElectronicsBoard.TabIndex = 0;
            this.labelElectronicsBoard.Text = "Electronics Board";
            // 
            // SelfCheckPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxTestElectronics);
            this.Controls.Add(this.groupBoxCamera);
            this.Name = "SelfCheckPanel";
            this.Size = new System.Drawing.Size(614, 394);
            this.Enter += new System.EventHandler(this.SelfCheckPanel_Enter);
            this.groupBoxCamera.ResumeLayout(false);
            this.groupBoxCamera.PerformLayout();
            this.groupBoxTestElectronics.ResumeLayout(false);
            this.groupBoxTestElectronics.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCamera;
        private System.Windows.Forms.TextBox textBoxOutputCameraStatus;
        private System.Windows.Forms.TextBox textBoxInputCameraStatus;
        private System.Windows.Forms.Button buttonCheckOutputCamera;
        private System.Windows.Forms.Button buttonCheckInputCamera;
        private System.Windows.Forms.Label labelOutputCamera;
        private System.Windows.Forms.Label labelInputCamera;
        private System.Windows.Forms.GroupBox groupBoxTestElectronics;
        private System.Windows.Forms.TextBox textBoxLCRMeterStatus;
        private System.Windows.Forms.TextBox textBoxElectronicsBoardStatus;
        private System.Windows.Forms.Button buttonCheckLCRMeter;
        private System.Windows.Forms.Button buttonCheckElectronicsBoard;
        private System.Windows.Forms.Label labelLCRMeter;
        private System.Windows.Forms.Label labelElectronicsBoard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFirmwareVersion;
        private System.Windows.Forms.Button checkFirmwareVersion;
    }
}
