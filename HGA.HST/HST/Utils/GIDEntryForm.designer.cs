namespace Seagate.AAS.HGA.HST.Utils
{
    partial class GIDEntryForm
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
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPassword = new Seagate.AAS.UI.TouchscreenTextBox();
            this.txtGID = new Seagate.AAS.UI.TouchscreenTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Controls.Add(this.btnLogin);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 105);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(316, 67);
            this.panel2.TabIndex = 3;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.BackColor = System.Drawing.SystemColors.Control;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(197, 15);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 40);
            this.buttonCancel.TabIndex = 50;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogin.BackColor = System.Drawing.SystemColors.Control;
            this.btnLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnLogin.Location = new System.Drawing.Point(29, 15);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(100, 40);
            this.btnLogin.TabIndex = 40;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Employee GID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 33;
            this.label1.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.AlphaNumOnly = false;
            this.txtPassword.BackColor = System.Drawing.Color.White;
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.FormTitle = "Enter Text";
            this.txtPassword.Location = new System.Drawing.Point(111, 73);
            this.txtPassword.MaxLength = 30;
            this.txtPassword.MinLength = 0;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.NoWhiteSpace = false;
            this.txtPassword.OnlyCaps = false;
            this.txtPassword.Size = new System.Drawing.Size(199, 22);
            this.txtPassword.TabIndex = 34;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtGID
            // 
            this.txtGID.AlphaNumOnly = false;
            this.txtGID.BackColor = System.Drawing.Color.White;
            this.txtGID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGID.FormTitle = "Enter Text";
            this.txtGID.Location = new System.Drawing.Point(111, 42);
            this.txtGID.MaxLength = 10;
            this.txtGID.MinLength = 0;
            this.txtGID.Name = "txtGID";
            this.txtGID.NoWhiteSpace = false;
            this.txtGID.OnlyCaps = false;
            this.txtGID.Size = new System.Drawing.Size(199, 22);
            this.txtGID.TabIndex = 32;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(286, 16);
            this.label2.TabIndex = 35;
            this.label2.Text = "Please login to proceed with system run.";
            // 
            // GIDEntryForm
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(316, 172);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtGID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GIDEntryForm";
            this.Text = "Employee GID Validation";
            this.Load += new System.EventHandler(this.GIDEntryForm_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label4;
        public Seagate.AAS.UI.TouchscreenTextBox txtGID;
        private System.Windows.Forms.Label label1;
        public AAS.UI.TouchscreenTextBox txtPassword;
        private System.Windows.Forms.Label label2;
    }
}