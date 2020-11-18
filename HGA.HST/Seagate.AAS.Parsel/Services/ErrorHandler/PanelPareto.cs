using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;


namespace Seagate.AAS.Parsel.Services
{
    public partial class PanelPareto : UserControl
    {
        // Implements the manual sorting of items by columns.
        class ListViewItemComparer : IComparer
        {
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                switch (col)
                {
                    case 0:				// Source string
                    case 1:				// Message string
                        return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                    case 2:				// count
                    case 3:				// total time
                    case 4:				// average time
                    case 5:				// min time
                    case 6:				// max time
                        return String.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
                }
                // column must be either Source or Message, sort alphabetically
                return String.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
            }
        }

        public PanelPareto()
        {
            InitializeComponent();
            //PopulateListView();
            toolTip1.SetToolTip(this.buttonLoadDataFile, "Load previous error summary data file (.dat) and display on screen");
            toolTip1.SetToolTip(this.buttonSaveCSVFile, "Save current displayed tab to csv file");
            toolTip1.SetToolTip(this.buttonRefresh, "Refresh/Show current date error summary");
        }
        private DateTime DisplayedErrorTableDateTime;

        private void PopulateListView()
        {
            listViewAllWc.Items.Clear();

            IDictionaryEnumerator myEnumerator = Services.ServiceManager.ErrorHandler.ErrorSummary.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                ListViewItem item = new ListViewItem(((MessageRecord)myEnumerator.Value).source);
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).text);
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).Count());
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).TotalTime());
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).AverageTime());
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).MinTime());
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).MaxTime());
                listViewAllWc.Items.Add(item);

                //listView.Items.Add(((MessageRecord)myEnumerator.Value).source);
                //int n = listView.Items.Count - 1;
                //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).text);
                //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).Count());
                //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).TotalTime());
                //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).AverageTime());
                //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).MinTime());
                //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).MaxTime());
            }
            listViewAllWc.Columns[0].Width = -1;		// size Source to longest string
            listViewAllWc.Columns[1].Width = -1;		// size Message to longest string
            listViewAllWc.Columns[2].Width = -2;		// autosize Count to longest string
            listViewAllWc.Columns[3].Width = -2;		// autosize Total Time to longest string
            listViewAllWc.Columns[4].Width = -2;		// autosize Average Time to longest string
            listViewAllWc.Columns[5].Width = -2;		// autosize Min Time to longest string
            listViewAllWc.Columns[6].Width = -2;		// autosize Max Time to longest string

            // do initial sort based on count of message occurrences (column 2)
            ColumnClickEventArgs a = new ColumnClickEventArgs(2);
            listView_ColumnClick(this, a);

        }

        private Hashtable GetWorkcellListedErrorTable(Hashtable errorTable)
        {
            Hashtable workcellListedErrorTable = new Hashtable();
            IDictionaryEnumerator myEnumerator = errorTable.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                string WC = ((MessageRecord)myEnumerator.Value).source;
                if (WC.Contains("_"))
                    WC = WC.Substring(0,WC.IndexOf("_"));

                // Create subErrorTable with workcell name as key
                if (!workcellListedErrorTable.ContainsKey(WC))
                {
                    workcellListedErrorTable.Add(WC, new Hashtable());
                }

                // Add MessageRecord and it's key to subErrortable
                ((Hashtable)workcellListedErrorTable[WC]).Add(myEnumerator.Key, myEnumerator.Value);
            }
            return workcellListedErrorTable;
        }

        private void RemoveSubParetoTab()
        {
            this.SuspendLayout();
            int tabCount  = tabControl1.Controls.Count;
            if (tabCount > 1)
            {
                for (int i = tabCount; i > 1 ; i-- )
                {
                    tabControl1.Controls.RemoveAt(i-1);
                }
            }
            this.ResumeLayout();
        }

        private void CreateUIPerWorkcell(Hashtable errorTable)
        {
            Hashtable wcListedErrorTable = GetWorkcellListedErrorTable(errorTable);
            if (wcListedErrorTable.Count > 1)
            {
                IDictionaryEnumerator myEnumerator = wcListedErrorTable.GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    AddStandardParetoTab((string)myEnumerator.Key, (Hashtable)myEnumerator.Value);
                }
            }
        }

        private void AddStandardParetoTab(string tabName, Hashtable subErrorTable)
        {
            this.SuspendLayout();
            TabPage tabPage = new TabPage(tabName);
            tabPage.Name = tabName;

            ColumnHeader col1 = (ColumnHeader)columnHeader1.Clone();
            ColumnHeader col2 = (ColumnHeader)columnHeader2.Clone();
            ColumnHeader col3 = (ColumnHeader)columnHeader3.Clone();
            ColumnHeader col4 = (ColumnHeader)columnHeader4.Clone();
            ColumnHeader col5 = (ColumnHeader)columnHeader5.Clone();
            ColumnHeader col6 = (ColumnHeader)columnHeader6.Clone();
            ColumnHeader col7 = (ColumnHeader)columnHeader7.Clone();

            ColumnHeader[] columnHeader = new ColumnHeader[] {
            col1,
            col2,
            col3,
            col4,
            col5,
            col6,
            col7
            };
            
            ListView listView = new ListView();
            listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            listView.AllowColumnReorder = true;
            listView.AutoArrange = false;
            listView.Columns.AddRange(columnHeader);
            listView.Dock = System.Windows.Forms.DockStyle.Fill;
            listView.GridLines = true;
            listView.Location = new System.Drawing.Point(3, 3);
            listView.Name = "listViewAllWc";
            listView.Size = new System.Drawing.Size(664, 395);
            listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            listView.TabIndex = 1;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = System.Windows.Forms.View.Details;
            listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);

            IDictionaryEnumerator myEnumerator = subErrorTable.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                ListViewItem item = new ListViewItem(((MessageRecord)myEnumerator.Value).source);
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).text);
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).Count());
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).TotalTime());
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).AverageTime());
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).MinTime());
                item.SubItems.Add(((MessageRecord)myEnumerator.Value).MaxTime());
                listView.Items.Add(item);
            }

            tabPage.Controls.Add(listView);
            tabControl1.Controls.Add(tabPage);
            this.ResumeLayout();
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Set the ListViewItemSorter property to a new ListViewItemComparer object.
            this.listViewAllWc.ListViewItemSorter = new ListViewItemComparer(e.Column);
            // Call the sort method to manually sort the column based on the ListViewItemComparer implementation.
            listViewAllWc.Sort();
        }

        public void RefreshPareto()
        {
            DateTime tmpDateTime = new DateTime();
            tmpDateTime = Services.ServiceManager.ErrorHandler.ErrorTableStartDateTime.AddHours(-6);
            DisplayedErrorTableDateTime = tmpDateTime;

            labelDate.Text = string.Format("Error summary from {0} 6:00am to {1} 6.00am", tmpDateTime.ToString("dd-MMM-yy"), tmpDateTime.AddDays(1).ToString("dd-MMM-yy"));
            PopulateListView();
            RemoveSubParetoTab();
            CreateUIPerWorkcell(Services.ServiceManager.ErrorHandler.ErrorSummary);
        }

        private void buttonLoadDataFile_Click(object sender, EventArgs e)
        {

            openFileDialog1.Filter = "Error data files (*.dat)|*.dat";
            openFileDialog1.Title = "Please select Error data file";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                if (File.Exists(filePath))
                    LoadPaletoFromFile(filePath);
                else
                    MessageBox.Show("Selected file does not exist");
            }
        }

        private void LoadPaletoFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            FileStream fs = new FileStream(filePath, FileMode.Open);
            try
            {
                Hashtable tmpErrorTable = null;
                DateTime startDateTime = new DateTime();
                BinaryFormatter bf = new BinaryFormatter();
                startDateTime = (DateTime)bf.Deserialize(fs);
                tmpErrorTable = (Hashtable)bf.Deserialize(fs);

                ColumnClickEventArgs a = new ColumnClickEventArgs(0);
                listView_ColumnClick(this, a);
                
                listViewAllWc.Items.Clear();
                IDictionaryEnumerator myEnumerator = tmpErrorTable.GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    ListViewItem item = new ListViewItem(((MessageRecord)myEnumerator.Value).source);
                    item.SubItems.Add(((MessageRecord)myEnumerator.Value).text);
                    item.SubItems.Add(((MessageRecord)myEnumerator.Value).Count());
                    item.SubItems.Add(((MessageRecord)myEnumerator.Value).TotalTime());
                    item.SubItems.Add(((MessageRecord)myEnumerator.Value).AverageTime());
                    item.SubItems.Add(((MessageRecord)myEnumerator.Value).MinTime());
                    item.SubItems.Add(((MessageRecord)myEnumerator.Value).MaxTime());
                    listViewAllWc.Items.Add(item);

                    //listView.Items.Add(((MessageRecord)myEnumerator.Value).source);
                    //int n = listView.Items.Count - 1;
                    //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).text);
                    //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).Count());
                    //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).TotalTime());
                    //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).AverageTime());
                    //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).MinTime());
                    //listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).MaxTime());
                }
                listViewAllWc.Columns[0].Width = -1;		// size Source to longest string
                listViewAllWc.Columns[1].Width = -1;		// size Message to longest string
                listViewAllWc.Columns[2].Width = -2;		// autosize Count to longest string
                listViewAllWc.Columns[3].Width = -2;		// autosize Total Time to longest string
                listViewAllWc.Columns[4].Width = -2;		// autosize Average Time to longest string
                listViewAllWc.Columns[5].Width = -2;		// autosize Min Time to longest string
                listViewAllWc.Columns[6].Width = -2;		// autosize Max Time to longest string

                // do initial sort based on count of message occurrences (column 2)
                a = new ColumnClickEventArgs(2);
                listView_ColumnClick(this, a);

                startDateTime.AddHours(-6);
                DisplayedErrorTableDateTime = startDateTime;
                labelDate.Text = string.Format("Loaded from File\nError summary from {0} 6:00am to {1} 6.00am", startDateTime.ToString("dd-MMM-yy"), startDateTime.AddDays(1).ToString("dd-MMM-yy"));

                RemoveSubParetoTab();
                CreateUIPerWorkcell(tmpErrorTable);
            }
            catch
            {

            }
            finally
            {
                fs.Close();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshPareto();
        }

        private void ExportToCSV()
        {
            saveFileDialog1.Title = "Save cvs file";
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog1.FileName = string.Format("ErrorTable{0}{1}.csv", tabControl1.SelectedTab.Text, DisplayedErrorTableDateTime.ToString("yyyyMMdd"));
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;

                string header = columnHeader1.Text + "," +
                    columnHeader2.Text + "," +
                    columnHeader3.Text + "," +
                    columnHeader4.Text + "," +
                    columnHeader5.Text + "," +
                    columnHeader6.Text + "," +
                    columnHeader7.Text;
                string item;
                try
                {
                    ListView tmpListview = (ListView)tabControl1.SelectedTab.Controls[0];

                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine(header);
                        for (int i = 0; i < tmpListview.Items.Count; i++)
                        {
                            item = "";
                            for (int j = 0; j < tmpListview.Items[i].SubItems.Count; j++)
                            {
                                item += tmpListview.Items[i].SubItems[j].Text;
                                item += ",";
                            }
                            item = item.Replace("\n", " ");
                            item = item.Replace("\r", " ");
                            sw.WriteLine(item);
                        }
                    }
                }
                catch
                {
                	
                }
            }
        }

        private void buttonSaveCSVFile_Click(object sender, EventArgs e)
        {
            ExportToCSV();
        }
    }
}
