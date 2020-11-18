namespace DesktopTester.UserControls
{
    partial class UserPasswordChangeForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code COMPortSettingsEditor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserPasswordChangeForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblServoCalibrationUpDownTabSelection = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtOldPass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNewPass1 = new System.Windows.Forms.TextBox();
            this.txtNewPass2 = new System.Windows.Forms.TextBox();
            this.errorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(16, 180);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(123, 33);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(143, 180);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(137, 33);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblServoCalibrationUpDownTabSelection
            // 
            this.lblServoCalibrationUpDownTabSelection.AutoSize = true;
            this.lblServoCalibrationUpDownTabSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServoCalibrationUpDownTabSelection.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblServoCalibrationUpDownTabSelection.Location = new System.Drawing.Point(12, 14);
            this.lblServoCalibrationUpDownTabSelection.Name = "lblServoCalibrationUpDownTabSelection";
            this.lblServoCalibrationUpDownTabSelection.Size = new System.Drawing.Size(37, 16);
            this.lblServoCalibrationUpDownTabSelection.TabIndex = 2;
            this.lblServoCalibrationUpDownTabSelection.Text = "User";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(55, 13);
            this.txtUser.Name = "txtUser";
            this.txtUser.ReadOnly = true;
            this.txtUser.Size = new System.Drawing.Size(225, 20);
            this.txtUser.TabIndex = 3;
            // 
            // txtOldPass
            // 
            this.txtOldPass.Location = new System.Drawing.Point(16, 69);
            this.txtOldPass.Name = "txtOldPass";
            this.txtOldPass.Size = new System.Drawing.Size(264, 20);
            this.txtOldPass.TabIndex = 4;
            this.txtOldPass.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Old Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(12, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "New Password";
            // 
            // txtNewPass1
            // 
            this.txtNewPass1.Location = new System.Drawing.Point(16, 119);
            this.txtNewPass1.Name = "txtNewPass1";
            this.txtNewPass1.Size = new System.Drawing.Size(264, 20);
            this.txtNewPass1.TabIndex = 6;
            this.txtNewPass1.UseSystemPasswordChar = true;
            // 
            // txtNewPass2
            // 
            this.txtNewPass2.Location = new System.Drawing.Point(16, 145);
            this.txtNewPass2.Name = "txtNewPass2";
            this.txtNewPass2.Size = new System.Drawing.Size(264, 20);
            this.txtNewPass2.TabIndex = 8;
            this.txtNewPass2.UseSystemPasswordChar = true;
            this.txtNewPass2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNewPass2_KeyPress);
            // 
            // errorToolTip
            // 
            this.errorToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Error;
            this.errorToolTip.ToolTipTitle = "Cannot Set New Password";
            // 
            // UserPasswordChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 224);
            this.Controls.Add(this.txtNewPass2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNewPass1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOldPass);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.lblServoCalibrationUpDownTabSelection);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserPasswordChangeForm";
            this.Text = "Change User Password";
            this.Load += new System.EventHandler(this.UserPasswordChangeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblServoCalibrationUpDownTabSelection;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtOldPass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNewPass1;
        private System.Windows.Forms.TextBox txtNewPass2;
        private System.Windows.Forms.ToolTip errorToolTip;
    }
}