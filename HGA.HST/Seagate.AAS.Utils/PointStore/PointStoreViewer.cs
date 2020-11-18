using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.Utils.PointStore
{
	/// <summary>
	/// Summary description for PointStoreViewer.
	/// </summary>
	public class PointStoreViewer : System.Windows.Forms.UserControl
	{

        // Nested declarations -------------------------------------------------
        enum ViewType
        {
        List,
        Statistic,
        Chart
        };

        // Member variables ----------------------------------------------------
        private PointStore pointStore;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private Seagate.AAS.UI.SimpleChart simpleChart1;
        private System.Windows.Forms.ComboBox comboBox1;
        public delegate void OnTreeViewSelectHandler();
        public event OnTreeViewSelectHandler OnTreeViewSelect;
        private PointStorage selectedPoint = null;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioViewList;
        private System.Windows.Forms.RadioButton radioStatistics;
        private System.Windows.Forms.RadioButton radioGraph;
        private System.Windows.Forms.ListBox listPoints;

        private string[] pointDisplayFilter = null;


        // Constructors & Finalizers -------------------------------------------

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PointStoreViewer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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

                
        // Properties ----------------------------------------------------------
        public PointStorage SelectedPoint
        {
            get
            {
                return selectedPoint;
            }
        }
		public RobotPoint SelectedHistoricalRobotPoint
		{
			get
			{
				if(listView1.SelectedItems.Count != 1)
					return null;
				else
					return (RobotPoint) listView1.SelectedItems[0].Tag;
			}
		}
		public PointStore PointStore
		{
			get
			{
				return this.pointStore;
			}
		}
        // Methods -------------------------------------------------------------
        public void AssignPointStore(PointStore pointStore)
        {
            this.pointStore = pointStore;
            pointStore.OnUpdate += new Seagate.AAS.Utils.PointStore.PointStore.UpdateHandler(UpdateDisplay);

//            foreach (PointStorage ps in pointStore.Points.Values)
//                ps.OnAddPoint += new Seagate.AAS.Utils.PointStore.PointStorage.OnAddPointHandler(UpdateDisplay);

            UpdatePointsList();

        }

        public void ClearFilter()
        {
            pointDisplayFilter = null;
            UpdatePointsList();
        }

        public void UpdateFilter(string filter)
        {
            filter = filter.Trim().ToLower();
            string[] myFilter = filter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (myFilter.GetLength(0) > 10)
                throw new Exception("PointStoreViewer can accept up to 10 filter");
            else if (myFilter.GetLength(0) < 1)
            {
                pointDisplayFilter = null;
            }
            else
            {
                pointDisplayFilter = myFilter;
            }

            UpdatePointsList();
        }

        // Internal methods ----------------------------------------------------
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.listView1 = new System.Windows.Forms.ListView();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.simpleChart1 = new Seagate.AAS.UI.SimpleChart();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioGraph = new System.Windows.Forms.RadioButton();
            this.radioStatistics = new System.Windows.Forms.RadioButton();
            this.radioViewList = new System.Windows.Forms.RadioButton();
            this.listPoints = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(192, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(8, 376);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // listView1
            // 
            this.listView1.AutoArrange = false;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(208, 72);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(256, 96);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Visible = false;
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "New Point";
            // 
            // simpleChart1
            // 
            this.simpleChart1.BoundaryLine.Color = System.Drawing.Color.Black;
            this.simpleChart1.BoundaryLine.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            this.simpleChart1.BoundaryLine.Width = 2;
            this.simpleChart1.ChartLine.Color = System.Drawing.Color.Black;
            this.simpleChart1.ChartLine.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            this.simpleChart1.ChartLine.Width = 1;
            this.simpleChart1.Location = new System.Drawing.Point(0, 0);
            this.simpleChart1.MeanLine.Color = System.Drawing.Color.Black;
            this.simpleChart1.MeanLine.Style = System.Drawing.Drawing2D.DashStyle.DashDot;
            this.simpleChart1.MeanLine.Width = 1;
            this.simpleChart1.Name = "simpleChart1";
            this.simpleChart1.Size = new System.Drawing.Size(0, 0);
            this.simpleChart1.Symbol.Color = System.Drawing.Color.Black;
            this.simpleChart1.Symbol.Size = 10;
            this.simpleChart1.Symbol.Style = Seagate.AAS.UI.SymbolShape.Circle;
            this.simpleChart1.TabIndex = 6;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Location = new System.Drawing.Point(248, 184);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(160, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.Visible = false;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioGraph);
            this.panel1.Controls.Add(this.radioStatistics);
            this.panel1.Controls.Add(this.radioViewList);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(200, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 48);
            this.panel1.TabIndex = 5;
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // radioGraph
            // 
            this.radioGraph.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioGraph.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioGraph.Location = new System.Drawing.Point(224, 8);
            this.radioGraph.Name = "radioGraph";
            this.radioGraph.Size = new System.Drawing.Size(80, 32);
            this.radioGraph.TabIndex = 2;
            this.radioGraph.Text = "Graph";
            this.radioGraph.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioGraph.CheckedChanged += new System.EventHandler(this.radioViewList_CheckedChanged);
            // 
            // radioStatistics
            // 
            this.radioStatistics.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioStatistics.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioStatistics.Location = new System.Drawing.Point(120, 8);
            this.radioStatistics.Name = "radioStatistics";
            this.radioStatistics.Size = new System.Drawing.Size(80, 32);
            this.radioStatistics.TabIndex = 1;
            this.radioStatistics.Text = "Statistics";
            this.radioStatistics.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioStatistics.CheckedChanged += new System.EventHandler(this.radioViewList_CheckedChanged);
            // 
            // radioViewList
            // 
            this.radioViewList.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioViewList.Checked = true;
            this.radioViewList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioViewList.Location = new System.Drawing.Point(24, 8);
            this.radioViewList.Name = "radioViewList";
            this.radioViewList.Size = new System.Drawing.Size(80, 32);
            this.radioViewList.TabIndex = 0;
            this.radioViewList.TabStop = true;
            this.radioViewList.Text = "List";
            this.radioViewList.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioViewList.CheckedChanged += new System.EventHandler(this.radioViewList_CheckedChanged);
            // 
            // listPoints
            // 
            this.listPoints.Dock = System.Windows.Forms.DockStyle.Left;
            this.listPoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listPoints.ItemHeight = 16;
            this.listPoints.Location = new System.Drawing.Point(0, 0);
            this.listPoints.Name = "listPoints";
            this.listPoints.Size = new System.Drawing.Size(192, 372);
            this.listPoints.Sorted = true;
            this.listPoints.TabIndex = 7;
            this.listPoints.SelectedIndexChanged += new System.EventHandler(this.listPoints_SelectedIndexChanged);
            // 
            // PointStoreViewer
            // 
            this.Controls.Add(this.simpleChart1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.listPoints);
            this.Name = "PointStoreViewer";
            this.Size = new System.Drawing.Size(544, 376);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void UpdatePointsList()
        {
            listPoints.Items.Clear();

            // iterate through each node add each point
            foreach (string keyName in pointStore.Points.Keys)
            {
                if (pointDisplayFilter != null)
                {
                    for (int i = 0; i < pointDisplayFilter.GetLength(0); i++)
                    {
                        if (keyName.ToLower().Contains(pointDisplayFilter[i]))
                        {
                            int index = listPoints.Items.Add(keyName);
                            break;
                        }
                    }
                    // create treeNode for ioNode
                }
                else
                {
                    int index = listPoints.Items.Add(keyName);
                }
            }

            if (listPoints.Items.Count > 0)
                listPoints.SelectedIndex = 0;

        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int index = comboBox1.SelectedIndex;

            PointStorage ps = (PointStorage) pointStore.Points[listPoints.SelectedItem];

            double[] chartValues = new double[ps.History.Count];
            for (int i=0; i<ps.History.Count; i++)
            {
                chartValues[i] = ((RobotPoint) ps.History[i]).Coordinate[index];
            }
            simpleChart1.Text = comboBox1.Items[index].ToString();
            simpleChart1.YValues = chartValues;

        }

        private void radioViewList_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateDisplay();
        
        }

        private void listPoints_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            selectedPoint = (PointStorage) pointStore.Points[listPoints.SelectedItem];

            // publish event
            if (OnTreeViewSelect != null)
            {
                OnTreeViewSelect();
            }

            listView1.BeginUpdate();
            if (radioViewList.Checked)
            {
                listView1.Clear();
                
                listView1.Columns.Add("Index"     ,   50, HorizontalAlignment.Center);
                listView1.Columns.Add("Time Stamp",  150, HorizontalAlignment.Center);
                for (int col=0; col<selectedPoint.Dimension; col++)
                {
                    listView1.Columns.Add(selectedPoint.DimensionName[col], 70, HorizontalAlignment.Right);
                }
                for (int index=0; index<selectedPoint.History.Count; index++)
                {
                    RobotPoint point = (RobotPoint) selectedPoint.History[index];
                    ListViewItem item = new ListViewItem(index.ToString());
                    item.Tag = point;
                    item.SubItems.Add(point.TimeStamp.ToString());                    
                    foreach (double val in point.Coordinate)
                    {
                        item.SubItems.Add(val.ToString("0.000"));                    
                    }
                    listView1.Items.Add(item);
                }
            }
            else if (radioStatistics.Checked)
            {
                listView1.Clear();
                listView1.Columns.Add("Dimension"   ,100, HorizontalAlignment.Center);
                listView1.Columns.Add("Average"     , 70, HorizontalAlignment.Center);
                listView1.Columns.Add("Stdev"       , 70, HorizontalAlignment.Center);
                listView1.Columns.Add("Min"         , 70, HorizontalAlignment.Center);
                listView1.Columns.Add("Max"         , 70, HorizontalAlignment.Center);

                for (int dimension=0; dimension<selectedPoint.Dimension; dimension++)
                {                   
                    ListViewItem item = new ListViewItem(selectedPoint.DimensionName[dimension]);
                    item.SubItems.Add(selectedPoint.Average[dimension].ToString("0.000"));
                    item.SubItems.Add(selectedPoint.Stdev[dimension].ToString("0.000"));
                    item.SubItems.Add(selectedPoint.Min[dimension].ToString("0.000"));
                    item.SubItems.Add(selectedPoint.Max[dimension].ToString("0.000"));
                    listView1.Items.Add(item);                
                }
            } 
            else if (radioGraph.Checked)
            {
                comboBox1.Items.Clear();
                foreach (string dimension in selectedPoint.DimensionName)
                {
                    comboBox1.Items.Add(dimension);
                }
                if (comboBox1.Items.Count>0)
                    comboBox1.SelectedIndex = 0;
            }      

            listView1.EndUpdate();

            if (radioGraph.Checked)
            {
                comboBox1.Dock = DockStyle.Top;
                comboBox1.Visible = true;

                simpleChart1.BringToFront();
                simpleChart1.Dock = DockStyle.Fill;
                simpleChart1.Visible = true;

                listView1.Dock = DockStyle.None;
                listView1.Visible = false;
            }
            else
            {
                comboBox1.Dock = DockStyle.None;
                comboBox1.Visible = false;

                simpleChart1.Dock = DockStyle.None;
                simpleChart1.Visible = false;

                listView1.Dock = DockStyle.Fill;
                listView1.Visible = true;
            }
        }

        private void panel1_Resize(object sender, System.EventArgs e)
        {
            int width = panel1.Width/3;
            radioViewList  .SetBounds(      0, 0, width, panel1.Height, BoundsSpecified.All);
            radioStatistics.SetBounds(  width, 0, width, panel1.Height, BoundsSpecified.All);
            radioGraph     .SetBounds(2*width, 0, width, panel1.Height, BoundsSpecified.All);
        }

    }
}
