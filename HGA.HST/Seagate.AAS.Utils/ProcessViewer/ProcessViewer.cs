using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.Utils.ProcessViewer
{
	/// <summary>
	/// Summary description for ProcessViewer.
	/// </summary>
	public class ProcessViewer : System.Windows.Forms.UserControl
	{
        //C#
        // Implements the manual sorting of items by columns.
        class ListViewItemComparer : IComparer 
        {
            private int col;
            private SortOrder order;
            public ListViewItemComparer() 
            {
                col=0;
                order = SortOrder.Ascending;
            }
            public ListViewItemComparer(int column, SortOrder order) 
            {
                col=column;
                this.order = order;
            }
            //C#
            public int Compare(object x, object y) 
            {
                int returnVal= -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                    ((ListViewItem)y).SubItems[col].Text);
                // Determine whether the sort order is descending.
                if(order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                        return returnVal;
            }
//            public int Compare(object x, object y) 
//            {
//                int returnVal;
//                bool isDate = false;
//
//                // Determine whether the type being compared is a date type.
//                try
//                {
//                    // Parse the two objects passed as a parameter as a DateTime.
//                    System.DateTime firstDate  = DateTime.Parse(((ListViewItem)x).SubItems[col].Text);
//                    System.DateTime secondDate = DateTime.Parse(((ListViewItem)y).SubItems[col].Text);
//                    returnVal = DateTime.Compare(firstDate, secondDate);
//                }
//                catch 
//                {
//                    returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
//                }
//
//                // Determine whether the sort order is descending.
//                if (order == SortOrder.Descending)
//                    // Invert the value returned by String.Compare.
//                    returnVal *= -1;
//                return returnVal;
//            }
        }

        private int sortColumn = -1;

        private System.Windows.Forms.ColumnHeader columnModuleName;
        private System.Windows.Forms.ColumnHeader columnVersion;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.ListView listViewModules;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox viewAll;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private System.Windows.Forms.ColumnHeader columnBuildDate;
        private System.Windows.Forms.ColumnHeader columnLocation;
        string [] filter;

		public ProcessViewer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

        public string [] Filter
        {
            set 
            {
                this.filter = value;   
            }
        }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.listViewModules = new System.Windows.Forms.ListView();
            this.columnModuleName = new System.Windows.Forms.ColumnHeader();
            this.columnVersion = new System.Windows.Forms.ColumnHeader();
            this.columnBuildDate = new System.Windows.Forms.ColumnHeader();
            this.columnSize = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.viewAll = new System.Windows.Forms.CheckBox();
            this.columnLocation = new System.Windows.Forms.ColumnHeader();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewModules
            // 
            this.listViewModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                              this.columnModuleName,
                                                                                              this.columnVersion,
                                                                                              this.columnBuildDate,
                                                                                              this.columnSize,
                                                                                              this.columnLocation});
            this.listViewModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewModules.FullRowSelect = true;
            this.listViewModules.Location = new System.Drawing.Point(0, 40);
            this.listViewModules.MultiSelect = false;
            this.listViewModules.Name = "listViewModules";
            this.listViewModules.Size = new System.Drawing.Size(504, 240);
            this.listViewModules.TabIndex = 0;
            this.listViewModules.View = System.Windows.Forms.View.Details;
            this.listViewModules.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewModules_ColumnClick);
            // 
            // columnModuleName
            // 
            this.columnModuleName.Text = "Module Name";
            this.columnModuleName.Width = 112;
            // 
            // columnVersion
            // 
            this.columnVersion.Text = "Version";
            this.columnVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnVersion.Width = 113;
            // 
            // columnBuildDate
            // 
            this.columnBuildDate.Text = "Build Date";
            this.columnBuildDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnBuildDate.Width = 137;
            // 
            // columnSize
            // 
            this.columnSize.Text = "Size (kb)";
            this.columnSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnSize.Width = 103;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.viewAll);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(504, 40);
            this.panel1.TabIndex = 1;
            // 
            // viewAll
            // 
            this.viewAll.Location = new System.Drawing.Point(312, 16);
            this.viewAll.Name = "viewAll";
            this.viewAll.Size = new System.Drawing.Size(176, 16);
            this.viewAll.TabIndex = 0;
            this.viewAll.Text = "View All Modules";
            this.viewAll.CheckedChanged += new System.EventHandler(this.viewAll_CheckedChanged);
            // 
            // columnLocation
            // 
            this.columnLocation.Text = "Location";
            // 
            // ProcessViewer
            // 
            this.Controls.Add(this.listViewModules);
            this.Controls.Add(this.panel1);
            this.Name = "ProcessViewer";
            this.Size = new System.Drawing.Size(504, 280);
            this.Load += new System.EventHandler(this.ProcessViewer_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void ProcessViewer_Load(object sender, System.EventArgs e)
        {
            if (this.DesignMode)
                return;

            LoadListView();
        }

        private void LoadListView()
        {
            listViewModules.Items.Clear();

            Process process = Process.GetCurrentProcess();
            foreach (ProcessModule module in process.Modules)
            {
                ListViewItem item = new ListViewItem(module.ModuleName);
                DateTime startDate = new DateTime(2000, 1, 1);
                DateTime buildDate;


                if (viewAll.Checked || PassFilter(module.ModuleName))
                {
                    string [] properties = new string[4];

                    if (module.FileVersionInfo.FileVersion != "")
                        properties[0] = module.FileVersionInfo.FileVersion;
                    else if (module.FileVersionInfo.ProductVersion != "")
                        properties[0] = module.FileVersionInfo.ProductVersion;
                    else
                        properties[0] = "";
                    
                    if (module.FileVersionInfo.FileBuildPart > 1000 )
                    {
                        buildDate = startDate.AddDays(module.FileVersionInfo.FileBuildPart);
                        buildDate = buildDate.AddSeconds(2*module.FileVersionInfo.FilePrivatePart);
                        properties[1] = buildDate.ToString("yy.MM.dd  hh:mm:ss");
                    }
                    else
                        properties[1] = "";
                    
                    properties[2] = string.Format("{0}", module.ModuleMemorySize/1000);
                    properties[3] = module.FileName;

                    item.SubItems.AddRange(properties);
                    listViewModules.Items.Add(item);
                }
            }
        }

        public bool PassFilter(string moduleName)
        {
            if (filter==null)
                return true;

            foreach (string test in filter)
            {
                if (moduleName.ToLower().StartsWith(test.ToLower()))
                    return true;
            
            }
            return false;
        }

        private void listViewModules_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listViewModules.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listViewModules.Sorting == SortOrder.Ascending)
                    listViewModules.Sorting = SortOrder.Descending;
                else
                    listViewModules.Sorting = SortOrder.Ascending;
            }


            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.listViewModules.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewModules.Sorting);

            // Call the sort method to manually sort.
            listViewModules.Sort();
        }

        private void viewAll_CheckedChanged(object sender, System.EventArgs e)
        {
            LoadListView();
        }
	}
}
