namespace Seagate.AAS.Parsel.Services
{
    partial class PanelPareto
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
            this.listViewAllWc = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.labelDate = new System.Windows.Forms.Label();
            this.buttonLoadDataFile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.buttonSaveCSVFile = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAllWorkcell = new System.Windows.Forms.TabPage();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabAllWorkcell.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewAllWc
            // 
            this.listViewAllWc.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewAllWc.AllowColumnReorder = true;
            this.listViewAllWc.AutoArrange = false;
            this.listViewAllWc.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.listViewAllWc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAllWc.GridLines = true;
            this.listViewAllWc.Location = new System.Drawing.Point(3, 3);
            this.listViewAllWc.Name = "listViewAllWc";
            this.listViewAllWc.Size = new System.Drawing.Size(664, 395);
            this.listViewAllWc.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewAllWc.TabIndex = 1;
            this.listViewAllWc.UseCompatibleStateImageBehavior = false;
            this.listViewAllWc.View = System.Windows.Forms.View.Details;
            this.listViewAllWc.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Source";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Message";
            this.columnHeader2.Width = 250;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Count";
            this.columnHeader3.Width = 51;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Total Time*";
            this.columnHeader4.Width = 75;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Average Time*";
            this.columnHeader5.Width = 80;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Min Time*";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Max Time*";
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.labelDate.Location = new System.Drawing.Point(3, 9);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(154, 20);
            this.labelDate.TabIndex = 2;
            this.labelDate.Text = "Error Summary Date";
            // 
            // buttonLoadDataFile
            // 
            this.buttonLoadDataFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoadDataFile.Location = new System.Drawing.Point(560, 3);
            this.buttonLoadDataFile.Name = "buttonLoadDataFile";
            this.buttonLoadDataFile.Size = new System.Drawing.Size(115, 34);
            this.buttonLoadDataFile.TabIndex = 3;
            this.buttonLoadDataFile.Text = "Load Dat File";
            this.buttonLoadDataFile.UseVisualStyleBackColor = true;
            this.buttonLoadDataFile.Click += new System.EventHandler(this.buttonLoadDataFile_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRefresh.Location = new System.Drawing.Point(439, 3);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(115, 34);
            this.buttonRefresh.TabIndex = 4;
            this.buttonRefresh.Text = "Refresh Current";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonSaveCSVFile
            // 
            this.buttonSaveCSVFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveCSVFile.Location = new System.Drawing.Point(318, 3);
            this.buttonSaveCSVFile.Name = "buttonSaveCSVFile";
            this.buttonSaveCSVFile.Size = new System.Drawing.Size(115, 34);
            this.buttonSaveCSVFile.TabIndex = 5;
            this.buttonSaveCSVFile.Text = "Save CSV File";
            this.buttonSaveCSVFile.UseVisualStyleBackColor = true;
            this.buttonSaveCSVFile.Click += new System.EventHandler(this.buttonSaveCSVFile_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabAllWorkcell);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.ItemSize = new System.Drawing.Size(74, 35);
            this.tabControl1.Location = new System.Drawing.Point(0, 64);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(678, 444);
            this.tabControl1.TabIndex = 6;
            // 
            // tabAllWorkcell
            // 
            this.tabAllWorkcell.Controls.Add(this.listViewAllWc);
            this.tabAllWorkcell.Location = new System.Drawing.Point(4, 39);
            this.tabAllWorkcell.Name = "tabAllWorkcell";
            this.tabAllWorkcell.Padding = new System.Windows.Forms.Padding(3);
            this.tabAllWorkcell.Size = new System.Drawing.Size(670, 401);
            this.tabAllWorkcell.TabIndex = 0;
            this.tabAllWorkcell.Text = "All WorkCells";
            this.tabAllWorkcell.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.UseAnimation = false;
            this.toolTip1.UseFading = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.Location = new System.Drawing.Point(512, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "*Time unit is minute:second";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // PanelPareto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonSaveCSVFile);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonLoadDataFile);
            this.Controls.Add(this.labelDate);
            this.Name = "PanelPareto";
            this.Size = new System.Drawing.Size(678, 508);
            this.tabControl1.ResumeLayout(false);
            this.tabAllWorkcell.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewAllWc;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Button buttonLoadDataFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button buttonSaveCSVFile;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabAllWorkcell;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
    }
}
