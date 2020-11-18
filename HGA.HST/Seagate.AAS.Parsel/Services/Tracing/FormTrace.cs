using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// Summary description for FormTrace.
	/// </summary>
	public class FormTrace : System.Windows.Forms.Form
    {
        private IContainer components;
		private System.Windows.Forms.Button OKBtn;

		private ArrayList checkedNodesList = new ArrayList();
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TreeView SourceTree;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ListView TraceListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem ProcessFlowViewMenuItem;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem TimeStampMenuItem;

		private Hashtable ht;
		public FormTrace(Hashtable ht)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			this.ht = ht;
			checkedNodesList.Clear();
			TraceListView.Enabled = false;
			// Populate the TreeView;
			PopulateTreeView();
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
            this.components = new System.ComponentModel.Container();
            this.OKBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TraceListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SourceTree = new System.Windows.Forms.TreeView();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.ProcessFlowViewMenuItem = new System.Windows.Forms.MenuItem();
            this.TimeStampMenuItem = new System.Windows.Forms.MenuItem();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKBtn
            // 
            this.OKBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKBtn.Location = new System.Drawing.Point(580, 379);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(88, 25);
            this.OKBtn.TabIndex = 3;
            this.OKBtn.Text = "OK";
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.TraceListView);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.SourceTree);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(680, 360);
            this.panel1.TabIndex = 6;
            // 
            // TraceListView
            // 
            this.TraceListView.AllowColumnReorder = true;
            this.TraceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.TraceListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TraceListView.FullRowSelect = true;
            this.TraceListView.GridLines = true;
            this.TraceListView.Location = new System.Drawing.Point(211, 0);
            this.TraceListView.Name = "TraceListView";
            this.TraceListView.Size = new System.Drawing.Size(469, 360);
            this.TraceListView.TabIndex = 8;
            this.TraceListView.UseCompatibleStateImageBehavior = false;
            this.TraceListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Source";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Message";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Time";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(208, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 360);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // SourceTree
            // 
            this.SourceTree.CheckBoxes = true;
            this.SourceTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.SourceTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SourceTree.ItemHeight = 16;
            this.SourceTree.Location = new System.Drawing.Point(0, 0);
            this.SourceTree.Name = "SourceTree";
            this.SourceTree.Size = new System.Drawing.Size(208, 360);
            this.SourceTree.TabIndex = 6;
            this.SourceTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.SourceTree_AfterCheck);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem5});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4});
            this.menuItem1.Text = "&File";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "E&xit";
            this.menuItem4.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ProcessFlowViewMenuItem,
            this.TimeStampMenuItem});
            this.menuItem5.Text = "&View";
            // 
            // ProcessFlowViewMenuItem
            // 
            this.ProcessFlowViewMenuItem.Index = 0;
            this.ProcessFlowViewMenuItem.Text = "&Process Flow View";
            this.ProcessFlowViewMenuItem.Click += new System.EventHandler(this.ProcessFlowViewMenuItem_Click);
            // 
            // TimeStampMenuItem
            // 
            this.TimeStampMenuItem.Index = 1;
            this.TimeStampMenuItem.Text = "&Time Stamp";
            this.TimeStampMenuItem.Click += new System.EventHandler(this.TimeStampMenuItem_Click);
            // 
            // FormTrace
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(680, 416);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.OKBtn);
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.Name = "FormTrace";
            this.ShowInTaskbar = false;
            this.Text = "Viper Traces";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void PopulateTreeView()
		{
			// node test for equality is not based on strings (2 different nodes with same string are
			// not equal!!!  so this loop goes through all of the tokens in the source strings and
			// populates the tree view based on a path like "viper.object.name.subname"
			// note that "viper.aaa.bbb.ccc" and "aaa.bbb.ccc" will both be separate branches on the tree!
			SourceTree.BeginUpdate();
			SourceTree.Nodes.Clear();
			Hashtable map = new Hashtable();
			map.Clear();

			IDictionaryEnumerator myEnumerator = ht.GetEnumerator();
			for(int j=0;myEnumerator.MoveNext();j++)
			{
				string source = myEnumerator.Key.ToString();
				char [] delim = { '.' };
				string [] tokens = source.Split(delim);
				TreeNodeCollection tnc = SourceTree.Nodes;
				int n;
				for(int i=0;i<tokens.Length;i++)
				{
					string s = tokens[i];
					if(tnc.Count > 0)
					{
						bool alreadyThere = false;
						for(n=0;n<tnc.Count;n++)
						{
							if(tnc[n].Text == s)
							{
								alreadyThere = true;
								break;
							}
						}
						if(!alreadyThere)
							n = tnc.Add(new TreeNode(tokens[i]));
					}
					else
						n = tnc.Add(new TreeNode(tokens[i]));
					tnc = tnc[n].Nodes;						
				}
			}
			SourceTree.EndUpdate();
			SourceTree.ExpandAll();
		}

		private void PopulateListView()
		{
			TraceListView.Clear();
			if(checkedNodesList.Count == 0)
			{
				TraceListView.Enabled = false;
				return;
			}
			else
				TraceListView.Enabled = true;

			if(ProcessFlowViewMenuItem.Checked)
				PopulateColumns_TimeView();
			else
				PopulateColumns();

			// autosize the columns
			foreach(ColumnHeader ch in TraceListView.Columns)
				ch.Width = -2;	// autosize
		}

		private void PopulateColumns()
		{
			// add the columns and determine max row count for the list
			int maxRow = 0;
			bool viewTimeStamp = TimeStampMenuItem.Checked;
			foreach(TreeNode node in checkedNodesList)
			{
				string header = node.Text;//String.Format("{0}.{1}",node.Parent.Text,node.Text);
				TraceListView.Columns.Add(header,150,HorizontalAlignment.Left);
				if(viewTimeStamp)
					TraceListView.Columns.Add("Timestamp",150,HorizontalAlignment.Left);

				ArrayList l = (ArrayList) ht[node.FullPath.Replace('\\','.')];
				if(l.Count > maxRow)
					maxRow = l.Count;
			}

			// create columns for each channel, don't interleave messages
			string item;
			int subitemcount = viewTimeStamp ? 2*checkedNodesList.Count - 1 : checkedNodesList.Count - 1;
			string [] subitems = new string[subitemcount];
			for(int i=0;i<maxRow;i++)
			{
				// finish the subitem strings
				item = "";
				for(int j=0;j<checkedNodesList.Count;j++)
				{
					ArrayList l = GetTraceList(j);
					if(j == 0)		// item and subitem[0]
					{
						if(i < l.Count)
						{
							TraceMessage tm = (TraceMessage)l[i];
							item = tm.Message;
							if(viewTimeStamp)
								subitems[0] = tm.Time;
						}
						else
						{
							item = "";
							if(viewTimeStamp)
								subitems[0] = "";
						}
					}
					else			// subitems only
					{
						int index = viewTimeStamp ? 2*j-1 : j-1;
						if(i < l.Count)
						{
							TraceMessage tm = (TraceMessage)l[i];
							subitems[index] = tm.Message;
							if(viewTimeStamp)
								subitems[index+1] = tm.Time;
						}
						else
						{
							subitems[index] = "";
							if(viewTimeStamp)
								subitems[index+1] = "";
						}
					}
				}
				TraceListView.Items.Add(item);
				TraceListView.Items[i].SubItems.AddRange(subitems);
			}
			// scroll to the last item in the list ?? 
			TraceListView.Items[maxRow-1].EnsureVisible();
		}

		private void PopulateColumns_TimeView()
		{
			bool viewTimeStamp = TimeStampMenuItem.Checked;
			// add the columns and determine row count for the list (total # of traces)
			int rowCount = 0;
			int i = 0;
			string [] channel = new string[checkedNodesList.Count];
			int [] index = new int[checkedNodesList.Count];
			foreach(TreeNode node in checkedNodesList)
			{
				string header = node.Text;//String.Format("{0}.{1}",node.Parent.Text,node.Text);
				TraceListView.Columns.Add(header,150,HorizontalAlignment.Left);
				if(viewTimeStamp)
					TraceListView.Columns.Add("Timestamp",150,HorizontalAlignment.Left);

				index[i] = 0;
				channel[i] = node.FullPath.Replace('\\','.');
				rowCount += ((ArrayList) ht[channel[i]]).Count;
				i++;
			}

			// now go through each message and put it in the list, oldest message first
			int x = 0;			// the column index for the next message to be added to the listview
			DateTime dt;
			TraceMessage tm = new TraceMessage("","");
			for(int row=0;row<rowCount;row++)
			{
				// find the next message to display
				dt = DateTime.Now;		// newer (greater) than all logged TraceMessages
				for(int col=0;col<checkedNodesList.Count;col++)
				{
					ArrayList l = (ArrayList) ht[channel[col]];
					if(index[col] < l.Count)			// verify channel has traces left to display
					{
						TraceMessage msg = (TraceMessage) l[index[col]];
						//if(DateTime.Compare(msg.TimeStamp,dt) < 0)	// determine if the message is older 
						if(msg.TimeStamp < dt)	// determine if the message is older 
						{
							dt = msg.TimeStamp;
							tm = msg;
							x = col;
						}
					}
				}
				++index[x];		// increment the channel index of this message

				// create the listview items & subitems to display the message
				string item = "";
				int subitemcount = viewTimeStamp ? 2*checkedNodesList.Count - 1 : checkedNodesList.Count - 1;
				string [] subitems = new string[subitemcount];

				for(int col=0;col<checkedNodesList.Count;col++)
				{
					if(col == 0)		// item (message) & subitem (timestamp)
					{
						if(col == x)	// message
						{
							item = tm.Message;
							if(viewTimeStamp)
								subitems[0] = tm.Time;
						}
						else			// null
						{
							item = "";
							if(viewTimeStamp)
								subitems[0] = "";
						}
					}
					else				// subitem (message) & subitem (timestamp)
					{
						int z = viewTimeStamp ? 2*col-1 : col-1;
						if(col == x)	// message
						{
							subitems[z] = tm.Message;
							if(viewTimeStamp)
								subitems[z+1] = tm.Time;
						}
						else			// null
						{
							subitems[z] = "";
							if(viewTimeStamp)
								subitems[z+1] = "";
						}
					}
				}
				TraceListView.Items.Add(item);
				TraceListView.Items[row].SubItems.AddRange(subitems);
			}
		}

		private ArrayList GetTraceList(int i)
		{
			TreeNode tn = (TreeNode)checkedNodesList[i];
			string s = tn.FullPath.Replace('\\','.');
			ArrayList l = (ArrayList) ht[s];
			return l;
		}

		private void OKBtn_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void SourceTree_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// verify that the node has traces
			string s = e.Node.FullPath.Replace('\\','.');
			if(ht.Contains(s))
			{
				if(e.Node.Checked)
					checkedNodesList.Add(e.Node);
				else
					checkedNodesList.Remove(e.Node);

				PopulateListView();
			}
		}

		private void TimeStampMenuItem_Click(object sender, System.EventArgs e)
		{
			TimeStampMenuItem.Checked = !TimeStampMenuItem.Checked;
			PopulateListView();
		}

		private void ProcessFlowViewMenuItem_Click(object sender, System.EventArgs e)
		{
			ProcessFlowViewMenuItem.Checked = !ProcessFlowViewMenuItem.Checked;
			PopulateListView();
		}
	}
}
