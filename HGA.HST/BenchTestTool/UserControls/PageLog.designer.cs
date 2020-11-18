namespace BenchTestsTool.UserControls
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
        /// the contents of this method with the code COMPortSettingsEditor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.listBox = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
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
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
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
            this.listBox.Size = new System.Drawing.Size(596, 501);
            this.listBox.TabIndex = 0;
            this.listBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyUp);
            this.listBox.Resize += new System.EventHandler(this.listBox_Resize);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanel.Controls.Add(this.listBox, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(596, 502);
            this.tableLayoutPanel.TabIndex = 2;
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}
