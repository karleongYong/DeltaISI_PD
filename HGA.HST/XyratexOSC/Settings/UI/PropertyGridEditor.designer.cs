namespace XyratexOSC.Settings.UI
{
    partial class PropertyGridEditor
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Device", 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyGridEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._cfgListView = new System.Windows.Forms.ListView();
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this._CfgView = new System.Windows.Forms.PropertyGrid();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._cfgListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._CfgView);
            this.splitContainer1.Size = new System.Drawing.Size(595, 500);
            this.splitContainer1.SplitterDistance = 155;
            this.splitContainer1.TabIndex = 0;
            // 
            // _cfgListView
            // 
            this._cfgListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this._cfgListView.BackColor = System.Drawing.SystemColors.ControlLight;
            this._cfgListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cfgListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cfgListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this._cfgListView.HideSelection = false;
            this._cfgListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this._cfgListView.LargeImageList = this._imageList;
            this._cfgListView.Location = new System.Drawing.Point(0, 0);
            this._cfgListView.Margin = new System.Windows.Forms.Padding(0);
            this._cfgListView.MultiSelect = false;
            this._cfgListView.Name = "_cfgListView";
            this._cfgListView.Scrollable = false;
            this._cfgListView.Size = new System.Drawing.Size(200, 423);
            this._cfgListView.TabIndex = 0;
            this._cfgListView.TileSize = new System.Drawing.Size(194, 36);
            this._cfgListView.UseCompatibleStateImageBehavior = false;
            this._cfgListView.View = System.Windows.Forms.View.Tile;
            this._cfgListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this._cfgListView_ItemSelectionChanged);
            // 
            // _imageList
            // 
            this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
            this._imageList.TransparentColor = System.Drawing.Color.Transparent;
            this._imageList.Images.SetKeyName(0, "SettingsIcon.png");
            // 
            // _CfgView
            // 
            this._CfgView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._CfgView.Location = new System.Drawing.Point(0, 0);
            this._CfgView.Name = "_CfgView";
            this._CfgView.Size = new System.Drawing.Size(755, 600);
            this._CfgView.TabIndex = 0;
            this._CfgView.ToolbarVisible = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(0, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(0, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(0, 0);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 0;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(0, 0);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 0;
            // 
            // PropertyGridEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "PropertyGridEditor";
            this.Size = new System.Drawing.Size(546, 457);
            this.Controls.SetChildIndex(this.flowLayoutPanel1, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView _cfgListView;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private new System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ImageList _imageList;
        private System.Windows.Forms.PropertyGrid _CfgView;
        private System.Windows.Forms.Button btnAccept;
        private new System.Windows.Forms.Button btnCancel;
        private new System.Windows.Forms.Button btnLoad;
    }
}
