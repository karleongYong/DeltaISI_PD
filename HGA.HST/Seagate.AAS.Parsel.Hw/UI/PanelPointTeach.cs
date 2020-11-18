using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Seagate.AAS.Utils.PointStore;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.UI;

namespace Seagate.AAS.Parsel.Hw.UI
{
	/// <summary>
	/// Summary description for PanelPointTeach.
	/// </summary>
	public class PanelPointTeach : System.Windows.Forms.UserControl
	{
        private Seagate.AAS.Utils.PointStore.PointStoreViewer pointStoreViewer1;
        private System.Windows.Forms.Button buttonTeach;
        private PointStore pointStore;
        private Hashtable axisMap;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonRestore;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Button buttonMove;

        public delegate void MoveToPointHandler(PointStorage point);
        private event MoveToPointHandler moveToPointHandler;


		public PanelPointTeach()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
            pointStoreViewer1.OnTreeViewSelect += new Seagate.AAS.Utils.PointStore.PointStoreViewer.OnTreeViewSelectHandler(OnPointStoreViewerTreeViewSelect);

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
			this.components = new System.ComponentModel.Container();
			this.pointStoreViewer1 = new Seagate.AAS.Utils.PointStore.PointStoreViewer();
			this.buttonTeach = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonRestore = new System.Windows.Forms.Button();
			this.buttonMove = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pointStoreViewer1
			// 
			this.pointStoreViewer1.Dock = System.Windows.Forms.DockStyle.Top;
			this.pointStoreViewer1.Location = new System.Drawing.Point(0, 0);
			this.pointStoreViewer1.Name = "pointStoreViewer1";
			this.pointStoreViewer1.Size = new System.Drawing.Size(472, 176);
			this.pointStoreViewer1.TabIndex = 0;
			// 
			// buttonTeach
			// 
			this.buttonTeach.Location = new System.Drawing.Point(8, 240);
			this.buttonTeach.Name = "buttonTeach";
			this.buttonTeach.Size = new System.Drawing.Size(104, 40);
			this.buttonTeach.TabIndex = 1;
			this.buttonTeach.Text = "Teach";
			this.buttonTeach.Click += new System.EventHandler(this.buttonTeach_Click);
			// 
			// listView1
			// 
			this.listView1.Dock = System.Windows.Forms.DockStyle.Right;
			this.listView1.FullRowSelect = true;
			this.listView1.Location = new System.Drawing.Point(136, 184);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(336, 200);
			this.listView1.TabIndex = 2;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 176);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(472, 8);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// timer1
			// 
			this.timer1.Interval = 250;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// buttonSave
			// 
			this.buttonSave.Location = new System.Drawing.Point(8, 288);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(104, 40);
			this.buttonSave.TabIndex = 4;
			this.buttonSave.Text = "Save";
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// buttonRestore
			// 
			this.buttonRestore.Location = new System.Drawing.Point(8, 336);
			this.buttonRestore.Name = "buttonRestore";
			this.buttonRestore.Size = new System.Drawing.Size(104, 40);
			this.buttonRestore.TabIndex = 5;
			this.buttonRestore.Text = "Restore";
			this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
			// 
			// buttonMove
			// 
			this.buttonMove.Location = new System.Drawing.Point(8, 192);
			this.buttonMove.Name = "buttonMove";
			this.buttonMove.Size = new System.Drawing.Size(104, 40);
			this.buttonMove.TabIndex = 6;
			this.buttonMove.Text = "Move to Point";
			this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
			// 
			// PanelPointTeach
			// 
			this.Controls.Add(this.buttonMove);
			this.Controls.Add(this.buttonRestore);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.buttonTeach);
			this.Controls.Add(this.pointStoreViewer1);
			this.Name = "PanelPointTeach";
			this.Size = new System.Drawing.Size(472, 384);
			this.VisibleChanged += new System.EventHandler(this.PanelPointTeach_VisibleChanged);
			this.ResumeLayout(false);

		}
		#endregion

        public void AssignPointStore(PointStore pointStore, Hashtable axisMap)
        {
            AssignPointStore(pointStore, axisMap, null);

        }
        
        public void AssignPointStore(PointStore pointStore, Hashtable axisMap, MoveToPointHandler moveToPointHandler)
        {
            this.pointStore = pointStore;
            this.axisMap    = axisMap;
            // only one handler allowed
            if (moveToPointHandler != null)
            {
                this.moveToPointHandler = moveToPointHandler;
                buttonMove.Enabled = true;
            }
            else
            {
                buttonMove.Enabled = false;
            }
            pointStoreViewer1.AssignPointStore(pointStore);
        }

        private void buttonTeach_Click(object sender, System.EventArgs e)
        {

            double[] position = new double[pointStoreViewer1.SelectedPoint.Dimension];

            for (int dimension=0; dimension<pointStoreViewer1.SelectedPoint.Dimension; dimension++)
            {
                string dimensionName = pointStoreViewer1.SelectedPoint.DimensionName[dimension];
                position[dimension] = ((IAxis) axisMap[dimensionName]).GetActualPosition();
            }

            pointStore.UpdatePointValue(pointStoreViewer1.SelectedPoint.Name, position);
        }

        private void CreateListView(PointStorage pointStorage)
        {
            listView1.Clear();

            if (pointStorage == null) return;

            listView1.Columns.Add("Dimension", 75, HorizontalAlignment.Left);
            listView1.Columns.Add("Axis", 75, HorizontalAlignment.Left);
            listView1.Columns.Add("Position", 100, HorizontalAlignment.Left);

            for (int dimension=0; dimension < pointStorage.Dimension; dimension++)
            {
                string dimensionName = pointStorage.DimensionName[dimension];

                ListViewItem item = new ListViewItem(dimensionName);
                if (axisMap.Contains(dimensionName))
                {
                    item.Tag = axisMap[dimensionName];           // axis object
                    item.SubItems.Add(((IAxis) axisMap[dimensionName]).Name);  // axis name
                    item.SubItems.Add("");                      
                }
                else
                {
                    item.Tag = null;           // axis object
                    item.SubItems.Add("unassigned axis");  // axis name
                    item.SubItems.Add("");                      
                }
                listView1.Items.Add(item);
            }
        }

        private void OnPointStoreViewerTreeViewSelect()
        {
            CreateListView(pointStoreViewer1.SelectedPoint);
        }

        private void UpdateListView()
        {
            if (pointStoreViewer1.SelectedPoint == null)
                return;

            foreach (ListViewItem item in listView1.Items) 
            {
                if (item.Tag is IAxis)
                {
                    double position = ((IAxis) item.Tag).GetActualPosition();
                    item.SubItems[2].Text = position.ToString("0.000");
                }
                else
                    item.SubItems[2].Text = "";
            }
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (this.Visible == false)
            {
                timer1.Enabled = false;
                return;
            }
            
            UpdateListView();
        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            this.pointStore.Save();
        
        }

        private void buttonRestore_Click(object sender, System.EventArgs e)
        {
            this.pointStore.Load();
        
        }

        private void buttonMove_Click(object sender, System.EventArgs e)
        {
            if (pointStoreViewer1.SelectedPoint == null)
                return;

            if (moveToPointHandler != null)
                moveToPointHandler(pointStoreViewer1.SelectedPoint);

        }

        private void PanelPointTeach_VisibleChanged(object sender, System.EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.timer1.Enabled = this.Visible;        
            }
        }

		private void listView1_ItemActivate(object sender, System.EventArgs e)
		{
			if(listView1.SelectedItems[0] == null)		// make sure a valid item was double clicked on
				return;

			int n = listView1.SelectedItems[0].Index;

			// copy current values into the position array
			double[] position = new double[pointStoreViewer1.SelectedPoint.Dimension];
			for(int dimension=0; dimension<pointStoreViewer1.SelectedPoint.Dimension; dimension++)
			{
				position[dimension] = ((PointStorage)pointStoreViewer1.SelectedPoint).CurrentPoint[dimension];
			}

			// now get a new value for the selected dimension
			NumEntryForm nef = new NumEntryForm(-10000.0,10000.0,position[n]);
			nef.ShowDialog();
			position[n] = nef.NumberEntered;

			pointStore.UpdatePointValue(pointStoreViewer1.SelectedPoint.Name, position);
		}
	}
}
