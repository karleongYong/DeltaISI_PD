namespace XyratexOSC.UI
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.txtAssemblies = new System.Windows.Forms.TextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblXyratex = new System.Windows.Forms.Label();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mainPanel.Controls.Add(this.lblCopyright);
            this.mainPanel.Controls.Add(this.txtAssemblies);
            this.mainPanel.Controls.Add(this.lblVersion);
            this.mainPanel.Controls.Add(this.btnOK);
            this.mainPanel.Controls.Add(this.lblProduct);
            this.mainPanel.Controls.Add(this.lblXyratex);
            this.mainPanel.Controls.Add(this.pbLogo);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(1, 1);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(659, 524);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPanel_MouseDown);
            // 
            // lblCopyright
            // 
            this.lblCopyright.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright.Location = new System.Drawing.Point(19, 113);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(365, 194);
            this.lblCopyright.TabIndex = 6;
            this.lblCopyright.Text = "© 2014 Xyratex. All rights reserved.";
            this.lblCopyright.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPanel_MouseDown);
            // 
            // txtAssemblies
            // 
            this.txtAssemblies.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtAssemblies.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtAssemblies.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAssemblies.Location = new System.Drawing.Point(13, 316);
            this.txtAssemblies.Multiline = true;
            this.txtAssemblies.Name = "txtAssemblies";
            this.txtAssemblies.ReadOnly = true;
            this.txtAssemblies.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAssemblies.Size = new System.Drawing.Size(635, 148);
            this.txtAssemblies.TabIndex = 5;
            this.txtAssemblies.WordWrap = false;
            // 
            // lblVersion
            // 
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI Light", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(100, 63);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(267, 35);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "Version 1.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblVersion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPanel_MouseDown);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(529, 470);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(120, 45);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Font = new System.Drawing.Font("Segoe UI Light", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProduct.Location = new System.Drawing.Point(159, 16);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(151, 47);
            this.lblProduct.TabIndex = 2;
            this.lblProduct.Text = "Apollo SI";
            this.lblProduct.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPanel_MouseDown);
            // 
            // lblXyratex
            // 
            this.lblXyratex.AutoSize = true;
            this.lblXyratex.Font = new System.Drawing.Font("Segoe UI Light", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXyratex.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(93)))), ((int)(((byte)(155)))));
            this.lblXyratex.Location = new System.Drawing.Point(30, 16);
            this.lblXyratex.Name = "lblXyratex";
            this.lblXyratex.Size = new System.Drawing.Size(128, 47);
            this.lblXyratex.TabIndex = 1;
            this.lblXyratex.Text = "Xyratex";
            this.lblXyratex.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPanel_MouseDown);
            // 
            // pbLogo
            // 
            this.pbLogo.Image = global::XyratexOSC.Properties.Resources.XyratexAppLogo;
            this.pbLogo.Location = new System.Drawing.Point(383, 5);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(284, 284);
            this.pbLogo.TabIndex = 0;
            this.pbLogo.TabStop = false;
            this.pbLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPanel_MouseDown);
            // 
            // AboutForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(93)))), ((int)(((byte)(155)))));
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(661, 526);
            this.Controls.Add(this.mainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AboutForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "About Xyratex PLI";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblXyratex;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TextBox txtAssemblies;
        private System.Windows.Forms.Label lblCopyright;
    }
}