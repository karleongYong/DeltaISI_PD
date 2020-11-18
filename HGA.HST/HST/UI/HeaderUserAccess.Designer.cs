namespace Seagate.AAS.HGA.HST.UI
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
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeaderUserAccess));
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelUserName = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.userIcon = new System.Windows.Forms.PictureBox();
            this.btnLogout = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonBypass = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Controls.Add(this.buttonBypass);
            this.panel1.Controls.Add(this.labelUserName);
            this.panel1.Controls.Add(this.txtUserName);
            this.panel1.Controls.Add(this.btnLogin);
            this.panel1.Controls.Add(this.userIcon);
            this.panel1.Controls.Add(this.btnLogout);
            this.panel1.Location = new System.Drawing.Point(3, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.MinimumSize = new System.Drawing.Size(110, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(125, 68);
            this.panel1.TabIndex = 2;
            // 
            // labelUserName
            // 
            this.labelUserName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.labelUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelUserName.Location = new System.Drawing.Point(5, 45);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(116, 19);
            this.labelUserName.TabIndex = 4;
            this.labelUserName.Text = "label1";
            this.labelUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtUserName
            // 
            this.txtUserName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(102)))), ((int)(((byte)(153)))));
            this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUserName.ForeColor = System.Drawing.SystemColors.Info;
            this.txtUserName.Location = new System.Drawing.Point(45, 29);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new System.Drawing.Size(75, 16);
            this.txtUserName.TabIndex = 3;
            this.txtUserName.Text = "Operator";
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnLogin.Image = ((System.Drawing.Image)(resources.GetObject("btnLogin.Image")));
            this.btnLogin.Location = new System.Drawing.Point(42, 2);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(28, 28);
            this.btnLogin.TabIndex = 0;
            this.toolTip.SetToolTip(this.btnLogin, "Login");
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            this.btnLogin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnLogin_MouseClick);
            // 
            // userIcon
            // 
            this.userIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.userIcon.Image = ((System.Drawing.Image)(resources.GetObject("userIcon.Image")));
            this.userIcon.Location = new System.Drawing.Point(5, 4);
            this.userIcon.Name = "userIcon";
            this.userIcon.Size = new System.Drawing.Size(37, 40);
            this.userIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.userIcon.TabIndex = 2;
            this.userIcon.TabStop = false;
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnLogout.Image = ((System.Drawing.Image)(resources.GetObject("btnLogout.Image")));
            this.btnLogout.Location = new System.Drawing.Point(69, 2);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(28, 28);
            this.btnLogout.TabIndex = 1;
            this.toolTip.SetToolTip(this.btnLogout, "Logout");
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.logoutButton_Click);
            // 
            // buttonBypass
            // 
            this.buttonBypass.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonBypass.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBypass.ForeColor = System.Drawing.Color.Maroon;
            this.buttonBypass.Location = new System.Drawing.Point(97, 2);
            this.buttonBypass.Name = "buttonBypass";
            this.buttonBypass.Size = new System.Drawing.Size(25, 28);
            this.buttonBypass.TabIndex = 5;
            this.buttonBypass.Text = "B";
            this.toolTip.SetToolTip(this.buttonBypass, "Bypass login");
            this.buttonBypass.UseVisualStyleBackColor = false;
            this.buttonBypass.Click += new System.EventHandler(this.buttonBypass_Click);
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
            this.Size = new System.Drawing.Size(128, 68);
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
        private System.Windows.Forms.Label labelUserName;
        public System.Windows.Forms.Button buttonBypass;
    }
}
