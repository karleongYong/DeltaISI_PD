namespace XyratexOSC.UI
{
    partial class NotifyBanner
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
            this.btnClose = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.BackgroundImage = global::XyratexOSC.Properties.Resources.RemoveIcon;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClose.Location = new System.Drawing.Point(645, 3);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2, 3, 6, 3);
            this.btnClose.MinimumSize = new System.Drawing.Size(29, 28);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(5, 0, 5, 1);
            this.btnClose.Size = new System.Drawing.Size(29, 28);
            this.btnClose.TabIndex = 0;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.flowLayout, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMessage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.pbIcon, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(680, 34);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // flowLayout
            // 
            this.flowLayout.AutoSize = true;
            this.flowLayout.BackColor = System.Drawing.Color.Transparent;
            this.flowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayout.Location = new System.Drawing.Point(643, 0);
            this.flowLayout.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayout.Name = "flowLayout";
            this.flowLayout.Size = new System.Drawing.Size(1, 33);
            this.flowLayout.TabIndex = 0;
            this.flowLayout.WrapContents = false;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoEllipsis = true;
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(36, 5);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(3, 5, 5, 5);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Padding = new System.Windows.Forms.Padding(6, 2, 0, 2);
            this.lblMessage.Size = new System.Drawing.Size(602, 23);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbIcon
            // 
            this.pbIcon.BackColor = System.Drawing.Color.Transparent;
            this.pbIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbIcon.Location = new System.Drawing.Point(0, 0);
            this.pbIcon.Margin = new System.Windows.Forms.Padding(0);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(33, 33);
            this.pbIcon.TabIndex = 2;
            this.pbIcon.TabStop = false;
            this.pbIcon.Visible = false;
            // 
            // NotifyBanner
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CausesValidation = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximumSize = new System.Drawing.Size(6000, 34);
            this.MinimumSize = new System.Drawing.Size(390, 34);
            this.Name = "NotifyBanner";
            this.Size = new System.Drawing.Size(680, 34);
            this.Load += new System.EventHandler(this.NotifyBanner_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.NotifyBanner_Paint);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// The flow layout panel for controls
        /// </summary>
        protected System.Windows.Forms.FlowLayoutPanel flowLayout;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.PictureBox pbIcon;
    }
}
