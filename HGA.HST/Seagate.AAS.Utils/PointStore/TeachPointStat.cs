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
	/// Summary description for UCTeachPointStat.
	/// </summary>
	public class TeachPointStat : System.Windows.Forms.UserControl
	{
        private System.Windows.Forms.DataGrid dataTeachHistoryStat;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TeachPointStat()
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
            this.dataTeachHistoryStat = new System.Windows.Forms.DataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.dataTeachHistoryStat)).BeginInit();
            this.SuspendLayout();
            // 
            // dataTeachHistoryStat
            // 
            this.dataTeachHistoryStat.AlternatingBackColor = System.Drawing.Color.Lavender;
            this.dataTeachHistoryStat.BackColor = System.Drawing.Color.WhiteSmoke;
            this.dataTeachHistoryStat.BackgroundColor = System.Drawing.Color.LightGray;
            this.dataTeachHistoryStat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataTeachHistoryStat.CaptionBackColor = System.Drawing.Color.LightSteelBlue;
            this.dataTeachHistoryStat.CaptionForeColor = System.Drawing.Color.MidnightBlue;
            this.dataTeachHistoryStat.CaptionText = "Statistics";
            this.dataTeachHistoryStat.DataMember = "";
            this.dataTeachHistoryStat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataTeachHistoryStat.FlatMode = true;
            this.dataTeachHistoryStat.Font = new System.Drawing.Font("Tahoma", 8F);
            this.dataTeachHistoryStat.ForeColor = System.Drawing.Color.MidnightBlue;
            this.dataTeachHistoryStat.GridLineColor = System.Drawing.Color.Gainsboro;
            this.dataTeachHistoryStat.GridLineStyle = System.Windows.Forms.DataGridLineStyle.None;
            this.dataTeachHistoryStat.HeaderBackColor = System.Drawing.Color.MidnightBlue;
            this.dataTeachHistoryStat.HeaderFont = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.dataTeachHistoryStat.HeaderForeColor = System.Drawing.Color.WhiteSmoke;
            this.dataTeachHistoryStat.LinkColor = System.Drawing.Color.Teal;
            this.dataTeachHistoryStat.Location = new System.Drawing.Point(0, 0);
            this.dataTeachHistoryStat.Name = "dataTeachHistoryStat";
            this.dataTeachHistoryStat.ParentRowsBackColor = System.Drawing.Color.Gainsboro;
            this.dataTeachHistoryStat.ParentRowsForeColor = System.Drawing.Color.MidnightBlue;
            this.dataTeachHistoryStat.ReadOnly = true;
            this.dataTeachHistoryStat.SelectionBackColor = System.Drawing.Color.CadetBlue;
            this.dataTeachHistoryStat.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.dataTeachHistoryStat.Size = new System.Drawing.Size(144, 92);
            this.dataTeachHistoryStat.TabIndex = 0;
            // 
            // UCTeachPointStat
            // 
            this.Controls.Add(this.dataTeachHistoryStat);
            this.Name = "UCTeachPointStat";
            this.Size = new System.Drawing.Size(144, 92);
            ((System.ComponentModel.ISupportInitialize)(this.dataTeachHistoryStat)).EndInit();
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
            dataTeachHistoryStat.CaptionText = "Teach Point: " + tPoint.Name;
            dataTeachHistoryStat.DataSource = tPoint.History.StatisticsTable;
            dataTeachHistoryStat.Refresh();
        }
    }
}
