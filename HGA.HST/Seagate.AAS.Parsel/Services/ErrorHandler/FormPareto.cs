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
//  [9/16/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// Summary description for FormPareto.
	/// </summary>
	public class FormPareto : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListView listView;

		// Implements the manual sorting of items by columns.
		class ListViewItemComparer : IComparer 
		{
			private int col;
			public ListViewItemComparer() 
			{
				col=0;
			}
			public ListViewItemComparer(int column) 
			{
				col=column;
			}
			public int Compare(object x, object y) 
			{
				switch(col)
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

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormPareto()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			PopulateListView();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listView
			// 
			this.listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.listView.AllowColumnReorder = true;
			this.listView.AutoArrange = false;
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.columnHeader1,
																					   this.columnHeader2,
																					   this.columnHeader3,
																					   this.columnHeader4,
																					   this.columnHeader5,
																					   this.columnHeader6,
																					   this.columnHeader7});
			this.listView.GridLines = true;
			this.listView.Location = new System.Drawing.Point(0, 0);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(640, 448);
			this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listView.TabIndex = 0;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
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
			this.columnHeader4.Text = "Total Time";
			this.columnHeader4.Width = 75;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Average Time";
			this.columnHeader5.Width = 80;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Min Time";
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Max Time";
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(560, 464);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(72, 32);
			this.button1.TabIndex = 1;
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// FormPareto
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(640, 504);
			this.ControlBox = false;
			this.Controls.Add(this.button1);
			this.Controls.Add(this.listView);
			this.Name = "FormPareto";
			this.Text = "My Error Report";
			this.ResumeLayout(false);

		}
		#endregion

		private void PopulateListView()
		{
			IDictionaryEnumerator myEnumerator = Services.ServiceManager.ErrorHandler.ErrorSummary.GetEnumerator();
			while(myEnumerator.MoveNext())
			{
				listView.Items.Add(((MessageRecord)myEnumerator.Value).source);
				int n = listView.Items.Count - 1;
				listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).text);
				listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).Count());
				listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).TotalTime());
				listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).AverageTime());
				listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).MinTime());
				listView.Items[n].SubItems.Add(((MessageRecord)myEnumerator.Value).MaxTime());
			}
			listView.Columns[0].Width = -1;		// size Source to longest string
			listView.Columns[1].Width = -1;		// size Message to longest string
			listView.Columns[2].Width = -2;		// autosize Count to longest string
			listView.Columns[3].Width = -2;		// autosize Total Time to longest string
			listView.Columns[4].Width = -2;		// autosize Average Time to longest string
			listView.Columns[5].Width = -2;		// autosize Min Time to longest string
			listView.Columns[6].Width = -2;		// autosize Max Time to longest string

			// do initial sort based on count of message occurrences (column 2)
			ColumnClickEventArgs a = new ColumnClickEventArgs(2);
			listView_ColumnClick(this,a);
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void listView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			// Set the ListViewItemSorter property to a new ListViewItemComparer object.
			this.listView.ListViewItemSorter = new ListViewItemComparer(e.Column);
			// Call the sort method to manually sort the column based on the ListViewItemComparer implementation.
			listView.Sort();
		}

	}
}
