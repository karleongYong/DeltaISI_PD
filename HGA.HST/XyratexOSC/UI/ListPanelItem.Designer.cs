namespace XyratexOSC.UI
{
    partial class ListPanelItem
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
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.panelSizer = new System.Windows.Forms.Panel();
            this.btnSizer = new XyratexOSC.UI.NoFocusCueButton();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.btnDelete = new XyratexOSC.UI.NoFocusCueButton();
            this.btnMove = new XyratexOSC.UI.NoFocusCueButton();
            this.panelSizer.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(2, 29);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(1, 40, 1, 3);
            this.propertyGrid.MinimumSize = new System.Drawing.Size(0, 2);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(323, 6);
            this.propertyGrid.TabIndex = 2;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyValueChanged);
            this.propertyGrid.SelectedObjectsChanged += new System.EventHandler(this.SelectedObjectsChanged);
            // 
            // panelSizer
            // 
            this.panelSizer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSizer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSizer.Controls.Add(this.btnSizer);
            this.panelSizer.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.panelSizer.Location = new System.Drawing.Point(0, 34);
            this.panelSizer.Margin = new System.Windows.Forms.Padding(0);
            this.panelSizer.Name = "panelSizer";
            this.panelSizer.Size = new System.Drawing.Size(327, 6);
            this.panelSizer.TabIndex = 1;
            this.panelSizer.Leave += new System.EventHandler(this.panelSizer_Leave);
            this.panelSizer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelSizer_MouseDown);
            this.panelSizer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelSizer_MouseMove);
            this.panelSizer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelSizer_MouseUp);
            // 
            // btnSizer
            // 
            this.btnSizer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSizer.AutoEllipsis = true;
            this.btnSizer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSizer.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.btnSizer.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowFrame;
            this.btnSizer.FlatAppearance.BorderSize = 0;
            this.btnSizer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnSizer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SkyBlue;
            this.btnSizer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSizer.Location = new System.Drawing.Point(-3, -2);
            this.btnSizer.Margin = new System.Windows.Forms.Padding(0);
            this.btnSizer.Name = "btnSizer";
            this.btnSizer.Size = new System.Drawing.Size(331, 8);
            this.btnSizer.TabIndex = 1;
            this.btnSizer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSizer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSizer.UseVisualStyleBackColor = true;
            this.btnSizer.Leave += new System.EventHandler(this.panelSizer_Leave);
            this.btnSizer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelSizer_MouseDown);
            this.btnSizer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelSizer_MouseMove);
            this.btnSizer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelSizer_MouseUp);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.Control;
            this.headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.headerPanel.Controls.Add(this.btnDelete);
            this.headerPanel.Controls.Add(this.btnMove);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(327, 33);
            this.headerPanel.TabIndex = 0;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowFrame;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SkyBlue;
            this.btnDelete.Image = global::XyratexOSC.Properties.Resources.RemoveIcon;
            this.btnDelete.Location = new System.Drawing.Point(296, 2);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
            this.btnDelete.MinimumSize = new System.Drawing.Size(26, 26);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(27, 27);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnMove
            // 
            this.btnMove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMove.AutoEllipsis = true;
            this.btnMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnMove.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.btnMove.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowFrame;
            this.btnMove.FlatAppearance.BorderSize = 0;
            this.btnMove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnMove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SkyBlue;
            this.btnMove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMove.Location = new System.Drawing.Point(-3, -2);
            this.btnMove.Margin = new System.Windows.Forms.Padding(0);
            this.btnMove.Name = "btnMove";
            this.btnMove.Padding = new System.Windows.Forms.Padding(30, 0, 30, 0);
            this.btnMove.Size = new System.Drawing.Size(331, 35);
            this.btnMove.TabIndex = 0;
            this.btnMove.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            this.btnMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnMove_MouseDown);
            this.btnMove.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnMove_MouseMove);
            this.btnMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnMove_MouseUp);
            // 
            // ListPanelItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.panelSizer);
            this.Controls.Add(this.propertyGrid);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(180, 40);
            this.Name = "ListPanelItem";
            this.Size = new System.Drawing.Size(327, 40);
            this.panelSizer.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private NoFocusCueButton btnDelete;
        private NoFocusCueButton btnMove;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Panel panelSizer;
        private System.Windows.Forms.Panel headerPanel;
        private NoFocusCueButton btnSizer;
    }
}
