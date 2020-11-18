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
	/// Summary description for UCTeacPointStore.
	/// </summary>
	public class TeacPointStore : System.Windows.Forms.UserControl
	{
        private TeachPointGrid ucTeachPointGrid1;
        private TeachPointHistoryGrid ucTeachPointHistory1;
        private TeachPointHistoryChart ucTeachPointHistoryChart1;
        private TeachPointStat ucTeachPointStat1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TeacPointStore()
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
            this.ucTeachPointGrid1 = new TeachPointGrid();
            this.ucTeachPointHistory1 = new TeachPointHistoryGrid();
            this.ucTeachPointHistoryChart1 = new TeachPointHistoryChart();
            this.ucTeachPointStat1 = new TeachPointStat();
            this.SuspendLayout();
            // 
            // ucTeachPointGrid1
            // 
            this.ucTeachPointGrid1.Location = new System.Drawing.Point(0, 0);
            this.ucTeachPointGrid1.Name = "ucTeachPointGrid1";
            this.ucTeachPointGrid1.Size = new System.Drawing.Size(296, 136);
            this.ucTeachPointGrid1.TabIndex = 0;
            // 
            // ucTeachPointHistory1
            // 
            this.ucTeachPointHistory1.Location = new System.Drawing.Point(0, 136);
            this.ucTeachPointHistory1.Name = "ucTeachPointHistory1";
            this.ucTeachPointHistory1.Size = new System.Drawing.Size(296, 120);
            this.ucTeachPointHistory1.TabIndex = 1;
            // 
            // ucTeachPointHistoryChart1
            // 
            this.ucTeachPointHistoryChart1.Location = new System.Drawing.Point(304, 40);
            this.ucTeachPointHistoryChart1.Name = "ucTeachPointHistoryChart1";
            this.ucTeachPointHistoryChart1.Size = new System.Drawing.Size(352, 304);
            this.ucTeachPointHistoryChart1.TabIndex = 2;
            // 
            // ucTeachPointStat1
            // 
            this.ucTeachPointStat1.Location = new System.Drawing.Point(0, 256);
            this.ucTeachPointStat1.Name = "ucTeachPointStat1";
            this.ucTeachPointStat1.Size = new System.Drawing.Size(296, 168);
            this.ucTeachPointStat1.TabIndex = 3;
            // 
            // UCTeacPointStore
            // 
            this.Controls.Add(this.ucTeachPointStat1);
            this.Controls.Add(this.ucTeachPointHistoryChart1);
            this.Controls.Add(this.ucTeachPointHistory1);
            this.Controls.Add(this.ucTeachPointGrid1);
            this.Name = "UCTeacPointStore";
            this.Size = new System.Drawing.Size(664, 424);
            this.ResumeLayout(false);

        }
		#endregion

        public void AssignPointStore(TeachPointStore pointStore)
        { 
            ucTeachPointGrid1.AssignPointStore(pointStore);
            ucTeachPointGrid1.AttachDisplays(ucTeachPointHistory1, ucTeachPointHistoryChart1, ucTeachPointStat1);
        }
    }
}
