namespace Seagate.AAS.HGA.HST.UserControls
{
    partial class PageLog
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listBox = new System.Windows.Forms.ListBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.checkDown = new System.Windows.Forms.CheckBox();
            this.pnlRecent = new System.Windows.Forms.Panel();
            this.txtLatest = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.pnlRecent.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel.SetColumnSpan(this.listBox, 2);
            this.listBox.ContextMenuStrip = this.contextMenuStrip;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox.FormattingEnabled = true;
            this.listBox.ItemHeight = 15;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.listBox.Name = "listBox";
            this.listBox.ScrollAlwaysVisible = true;
            this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox.Size = new System.Drawing.Size(596, 478);
            this.listBox.TabIndex = 0;
            this.listBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyUp);
            this.listBox.Resize += new System.EventHandler(this.listBox_Resize);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(103, 26);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanel.Controls.Add(this.listBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.checkDown, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.pnlRecent, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(596, 502);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // checkDown
            // 
            this.checkDown.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkDown.BackColor = System.Drawing.SystemColors.ControlLight;
            this.checkDown.Checked = true;
            this.checkDown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkDown.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.checkDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkDown.Image = global::Seagate.AAS.HGA.HST.Properties.Resources.down_enabled;
            this.checkDown.Location = new System.Drawing.Point(577, 479);
            this.checkDown.Margin = new System.Windows.Forms.Padding(0);
            this.checkDown.Name = "checkDown";
            this.checkDown.Size = new System.Drawing.Size(19, 23);
            this.checkDown.TabIndex = 2;
            this.toolTip.SetToolTip(this.checkDown, "Disable Auto Scroll");
            this.checkDown.UseVisualStyleBackColor = false;
            this.checkDown.CheckedChanged += new System.EventHandler(this.checkDown_CheckedChanged);
            // 
            // pnlRecent
            // 
            this.pnlRecent.BackColor = System.Drawing.Color.Green;
            this.pnlRecent.Controls.Add(this.txtLatest);
            this.pnlRecent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRecent.Location = new System.Drawing.Point(0, 479);
            this.pnlRecent.Margin = new System.Windows.Forms.Padding(0);
            this.pnlRecent.Name = "pnlRecent";
            this.pnlRecent.Size = new System.Drawing.Size(577, 23);
            this.pnlRecent.TabIndex = 3;
            // 
            // txtLatest
            // 
            this.txtLatest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLatest.BackColor = System.Drawing.Color.Green;
            this.txtLatest.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLatest.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLatest.ForeColor = System.Drawing.Color.White;
            this.txtLatest.Location = new System.Drawing.Point(4, 4);
            this.txtLatest.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            this.txtLatest.Name = "txtLatest";
            this.txtLatest.ReadOnly = true;
            this.txtLatest.Size = new System.Drawing.Size(570, 16);
            this.txtLatest.TabIndex = 2;
            this.txtLatest.WordWrap = false;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // PageLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "PageLog";
            this.Size = new System.Drawing.Size(596, 502);
            this.VisibleChanged += new System.EventHandler(this.PageLog_VisibleChanged);
            this.contextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.pnlRecent.ResumeLayout(false);
            this.pnlRecent.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.CheckBox checkDown;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Panel pnlRecent;
        private System.Windows.Forms.TextBox txtLatest;
    }
}
