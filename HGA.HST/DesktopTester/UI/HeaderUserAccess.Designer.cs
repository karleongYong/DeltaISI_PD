namespace DesktopTester.UI
{
    partial class HeaderUserAccess
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
        /// the contents of this method with the code COMPortSettingsEditor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeaderUserAccess));
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.userIcon = new System.Windows.Forms.PictureBox();
            this.btnLogout = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Controls.Add(this.txtUserName);
            this.panel1.Controls.Add(this.btnLogin);
            this.panel1.Controls.Add(this.userIcon);
            this.panel1.Controls.Add(this.btnLogout);
            this.panel1.Location = new System.Drawing.Point(3, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.MinimumSize = new System.Drawing.Size(110, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(120, 65);
            this.panel1.TabIndex = 2;
            // 
            // txtUserName
            // 
            this.txtUserName.BackColor = System.Drawing.SystemColors.Highlight;
            this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUserName.ForeColor = System.Drawing.Color.Black;
            this.txtUserName.Location = new System.Drawing.Point(59, 38);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new System.Drawing.Size(56, 16);
            this.txtUserName.TabIndex = 3;
            this.txtUserName.Text = "Monitor";
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnLogin.Image = ((System.Drawing.Image)(resources.GetObject("btnLogin.Image")));
            this.btnLogin.Location = new System.Drawing.Point(59, 4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(28, 28);
            this.btnLogin.TabIndex = 0;
            this.toolTip.SetToolTip(this.btnLogin, "Login");
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnLogin_MouseClick);
            // 
            // userIcon
            // 
            this.userIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.userIcon.Image = ((System.Drawing.Image)(resources.GetObject("userIcon.Image")));
            this.userIcon.Location = new System.Drawing.Point(5, 17);
            this.userIcon.Name = "userIcon";
            this.userIcon.Size = new System.Drawing.Size(48, 48);
            this.userIcon.TabIndex = 2;
            this.userIcon.TabStop = false;
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnLogout.Image = ((System.Drawing.Image)(resources.GetObject("btnLogout.Image")));
            this.btnLogout.Location = new System.Drawing.Point(89, 4);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(28, 28);
            this.btnLogout.TabIndex = 1;
            this.toolTip.SetToolTip(this.btnLogout, "Logout");
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.logoutButton_Click);
            // 
            // HeaderUserAccess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "HeaderUserAccess";
            this.Size = new System.Drawing.Size(123, 65);
            this.Load += new System.EventHandler(this.HeaderUserAccess_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button btnLogout;
        public System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.PictureBox userIcon;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
