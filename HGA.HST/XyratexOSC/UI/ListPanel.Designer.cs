namespace XyratexOSC.UI
{
    partial class ListPanel
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
            this.Panel = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.scrollBar = new System.Windows.Forms.VScrollBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel
            // 
            this.Panel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Panel.Controls.Add(this.btnAdd);
            this.Panel.Location = new System.Drawing.Point(0, 0);
            this.Panel.Margin = new System.Windows.Forms.Padding(0);
            this.Panel.Name = "Panel";
            this.Panel.Size = new System.Drawing.Size(350, 34);
            this.Panel.TabIndex = 1;
            this.Panel.ClientSizeChanged += new System.EventHandler(this.Panel_ClientSizeChanged);
            this.Panel.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.Panel_ControlAdded);
            this.Panel.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.Panel_ControlRemoved);
            // 
            // btnAdd
            // 
            this.btnAdd.Image = global::XyratexOSC.Properties.Resources.AddIcon;
            this.btnAdd.Location = new System.Drawing.Point(2, 2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(30, 30);
            this.btnAdd.TabIndex = 0;
            this.toolTip.SetToolTip(this.btnAdd, "Add New");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // scrollBar
            // 
            this.scrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.scrollBar.Enabled = false;
            this.scrollBar.Location = new System.Drawing.Point(350, 0);
            this.scrollBar.Name = "scrollBar";
            this.scrollBar.Size = new System.Drawing.Size(17, 514);
            this.scrollBar.TabIndex = 2;
            this.scrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollBar_Scroll);
            // 
            // ListPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.scrollBar);
            this.Controls.Add(this.Panel);
            this.Name = "ListPanel";
            this.Size = new System.Drawing.Size(367, 514);
            this.Resize += new System.EventHandler(this.ListPanel_Resize);
            this.Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel Panel;
        private System.Windows.Forms.VScrollBar scrollBar;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
