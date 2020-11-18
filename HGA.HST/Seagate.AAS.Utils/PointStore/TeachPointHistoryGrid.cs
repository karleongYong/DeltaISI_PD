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
	/// Summary description for UCTeachPointHistory.
	/// </summary>
	public class TeachPointHistoryGrid : System.Windows.Forms.UserControl
	{
        private System.Windows.Forms.DataGrid gridTPointHistory;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TeachPointHistoryGrid()
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
            this.gridTPointHistory = new System.Windows.Forms.DataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.gridTPointHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // gridTPointHistory
            // 
            this.gridTPointHistory.AlternatingBackColor = System.Drawing.Color.Lavender;
            this.gridTPointHistory.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gridTPointHistory.BackgroundColor = System.Drawing.Color.LightGray;
            this.gridTPointHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridTPointHistory.CaptionBackColor = System.Drawing.Color.LightSteelBlue;
            this.gridTPointHistory.CaptionForeColor = System.Drawing.Color.MidnightBlue;
            this.gridTPointHistory.CaptionText = "Teach point history";
            this.gridTPointHistory.DataMember = "";
            this.gridTPointHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTPointHistory.FlatMode = true;
            this.gridTPointHistory.Font = new System.Drawing.Font("Tahoma", 8F);
            this.gridTPointHistory.ForeColor = System.Drawing.Color.MidnightBlue;
            this.gridTPointHistory.GridLineColor = System.Drawing.Color.Gainsboro;
            this.gridTPointHistory.GridLineStyle = System.Windows.Forms.DataGridLineStyle.None;
            this.gridTPointHistory.HeaderBackColor = System.Drawing.Color.MidnightBlue;
            this.gridTPointHistory.HeaderFont = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.gridTPointHistory.HeaderForeColor = System.Drawing.Color.WhiteSmoke;
            this.gridTPointHistory.LinkColor = System.Drawing.Color.Teal;
            this.gridTPointHistory.Location = new System.Drawing.Point(0, 0);
            this.gridTPointHistory.Name = "gridTPointHistory";
            this.gridTPointHistory.ParentRowsBackColor = System.Drawing.Color.Gainsboro;
            this.gridTPointHistory.ParentRowsForeColor = System.Drawing.Color.MidnightBlue;
            this.gridTPointHistory.ReadOnly = true;
            this.gridTPointHistory.SelectionBackColor = System.Drawing.Color.CadetBlue;
            this.gridTPointHistory.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.gridTPointHistory.Size = new System.Drawing.Size(56, 48);
            this.gridTPointHistory.TabIndex = 0;
            // 
            // UCTeachPointHistory
            // 
            this.Controls.Add(this.gridTPointHistory);
            this.Name = "UCTeachPointHistory";
            this.Size = new System.Drawing.Size(56, 48);
            ((System.ComponentModel.ISupportInitialize)(this.gridTPointHistory)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

        public void Update(object subject)
        {
            if(subject is TeachPoint)
                AssignTeachPoint((TeachPoint)subject);
        }

        public void AssignTeachPoint(TeachPoint tPoint)
        {
            gridTPointHistory.CaptionText = "Teach Point: " + tPoint.Name;
            gridTPointHistory.DataSource = tPoint.History.HistoryTable;
            gridTPointHistory.Refresh();
        }
	}
}
