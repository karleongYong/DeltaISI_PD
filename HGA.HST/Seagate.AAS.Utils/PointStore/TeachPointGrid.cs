//
//  © Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [6/6/2006]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.Utils.PointStore
{
	/// <summary>
	/// Summary description for UCTeachPointGrid.
	/// </summary>
	public class TeachPointGrid : System.Windows.Forms.UserControl
	{
        // Member variables ----------------------------------------------------
        
        public delegate void SelectTeachPointHandler(object sender);
        public event SelectTeachPointHandler SelectEvent;
   
        TeachPointStore pointStore;
        private TeachPoint _currentPoint;

        // Constructors & Finalizers -------------------------------------------
        
        // Properties ----------------------------------------------------------
        
        public TeachPoint CurrentPoint
        { get { return _currentPoint; } }

        // Methods -------------------------------------------------------------
        
        public void AssignPointStore(TeachPointStore pointStore)
        { 
            this.pointStore = pointStore;
            gridTeachPoints.DataSource = pointStore.TeachPointTable;
            gridTeachPoints.Refresh();

            pointStore.PointStoreChanged += new TeachPointStore.PointStoreContentChangedEventHandler(pointStore_OnUpdate);
        }

        public void AttachDisplays(TeachPointHistoryGrid historyDisplay, 
            TeachPointHistoryChart historyChart, 
            TeachPointStat statDisplay)
        {
            if (historyDisplay != null)
                SelectEvent += new SelectTeachPointHandler(historyDisplay.Update);

            if (historyChart != null)
                SelectEvent += new SelectTeachPointHandler(historyChart.Update);

            if (statDisplay != null)
                SelectEvent += new SelectTeachPointHandler(statDisplay.Update);
        }

        public void SelectTeachPoint(TeachPoint point)
        {
            gridTeachPoints.UnSelect(gridTeachPoints.CurrentRowIndex);

            // find row number
            DataTable dt = (DataTable)gridTeachPoints.DataSource;
            int rowNum;
            for(rowNum = 0; rowNum < dt.Rows.Count; rowNum++)
            {
                DataRow row = dt.Rows[rowNum];
                string ptn = row[0].ToString();
                if (ptn == point.Name)
                    break;
            }
            if (rowNum < dt.Rows.Count)
            {
                //lastRowSelect = rowNum;
                gridTeachPoints.Select(rowNum);
                gridTeachPoints.CurrentRowIndex = rowNum;
                _currentPoint = point;
            }
        }

        // Form ----------------------------------------------------------------
        
        private System.Windows.Forms.DataGrid gridTeachPoints;
        
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TeachPointGrid()
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.gridTeachPoints = new System.Windows.Forms.DataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.gridTeachPoints)).BeginInit();
            this.SuspendLayout();
            // 
            // gridTeachPoints
            // 
            this.gridTeachPoints.AllowSorting = false;
            this.gridTeachPoints.BackgroundColor = System.Drawing.Color.LightGray;
            this.gridTeachPoints.CaptionText = "Teach Points";
            this.gridTeachPoints.DataMember = "";
            this.gridTeachPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTeachPoints.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.gridTeachPoints.Location = new System.Drawing.Point(0, 0);
            this.gridTeachPoints.Name = "gridTeachPoints";
            this.gridTeachPoints.ReadOnly = true;
            this.gridTeachPoints.Size = new System.Drawing.Size(56, 48);
            this.gridTeachPoints.TabIndex = 0;
            this.gridTeachPoints.CurrentCellChanged += new System.EventHandler(this.gridTeachPoinst_CurrentCellChanged);
            this.gridTeachPoints.Leave += new System.EventHandler(this.gridTeachPoints_Leave);
            // 
            // UCTeachPointGrid
            // 
            this.Controls.Add(this.gridTeachPoints);
            this.Name = "UCTeachPointGrid";
            this.Size = new System.Drawing.Size(56, 48);
            this.Load += new System.EventHandler(this.TeachPointGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridTeachPoints)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

        private void TeachPointGrid_Load(object sender, System.EventArgs e)
        {
        }

        private void pointStore_OnUpdate()
        {
            gridTeachPoinst_CurrentCellChanged(null, null);
        }

        private void gridTeachPoinst_CurrentCellChanged(object sender, System.EventArgs e)
        {
            // find teach point
            int rowNum = gridTeachPoints.CurrentCell.RowNumber;
            string tpName = gridTeachPoints[rowNum,0].ToString();
            _currentPoint = pointStore.GetPoint(tpName);

            if(SelectEvent != null) 
                SelectEvent(_currentPoint);
        }

        private void gridTeachPoints_Leave(object sender, System.EventArgs e)
        {
        
        }
	}
}
