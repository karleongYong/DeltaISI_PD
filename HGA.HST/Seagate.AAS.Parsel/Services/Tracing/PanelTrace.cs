using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// Summary description for PanelTrace.
	/// </summary>
	public class PanelTrace : System.Windows.Forms.UserControl
	{
		private System.ComponentModel.IContainer components;

		private Hashtable ht;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton OpenFileBtn;
		private System.Windows.Forms.ToolBarButton SaveFileBtn;
		private System.Windows.Forms.ToolBarButton SeparatorBtn1;
		private System.Windows.Forms.ToolBarButton ProcessViewBtn;
		private System.Windows.Forms.ToolBarButton ViewTimeStampBtn;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TreeView SourceTree;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ListView TraceListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.ToolBarButton RefreshTracesBtn;
		private ArrayList checkedNodesList = new ArrayList();

		public PanelTrace()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			
		}

		public void Initialize(Hashtable ht)
		{
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelTrace));
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.OpenFileBtn = new System.Windows.Forms.ToolBarButton();
			this.SaveFileBtn = new System.Windows.Forms.ToolBarButton();
			this.RefreshTracesBtn = new System.Windows.Forms.ToolBarButton();
			this.SeparatorBtn1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.ProcessViewBtn = new System.Windows.Forms.ToolBarButton();
			this.ViewTimeStampBtn = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.panel1 = new System.Windows.Forms.Panel();
			this.TraceListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.SourceTree = new System.Windows.Forms.TreeView();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolBar1
			// 
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.OpenFileBtn,
            this.SaveFileBtn,
            this.RefreshTracesBtn,
            this.SeparatorBtn1,
            this.toolBarButton1,
            this.ProcessViewBtn,
            this.ViewTimeStampBtn});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(536, 28);
			this.toolBar1.TabIndex = 10;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// OpenFileBtn
			// 
			this.OpenFileBtn.ImageIndex = 0;
			this.OpenFileBtn.Name = "OpenFileBtn";
			this.OpenFileBtn.ToolTipText = "Open Trace File";
			// 
			// SaveFileBtn
			// 
			this.SaveFileBtn.ImageIndex = 1;
			this.SaveFileBtn.Name = "SaveFileBtn";
			this.SaveFileBtn.ToolTipText = "Save Trace File";
			// 
			// RefreshTracesBtn
			// 
			this.RefreshTracesBtn.ImageIndex = 4;
			this.RefreshTracesBtn.Name = "RefreshTracesBtn";
			this.RefreshTracesBtn.ToolTipText = "Refresh Traces ";
			// 
			// SeparatorBtn1
			// 
			this.SeparatorBtn1.Name = "SeparatorBtn1";
			this.SeparatorBtn1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Name = "toolBarButton1";
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// ProcessViewBtn
			// 
			this.ProcessViewBtn.ImageIndex = 3;
			this.ProcessViewBtn.Name = "ProcessViewBtn";
			this.ProcessViewBtn.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.ProcessViewBtn.ToolTipText = "Process Flow View";
			// 
			// ViewTimeStampBtn
			// 
			this.ViewTimeStampBtn.ImageIndex = 2;
			this.ViewTimeStampBtn.Name = "ViewTimeStampBtn";
			this.ViewTimeStampBtn.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.ViewTimeStampBtn.ToolTipText = "Show Message Time Stamps";
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.imageList1.Images.SetKeyName(2, "");
			this.imageList1.Images.SetKeyName(3, "");
			this.imageList1.Images.SetKeyName(4, "");
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.TraceListView);
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.SourceTree);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 28);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(536, 308);
			this.panel1.TabIndex = 11;
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
			this.TraceListView.Location = new System.Drawing.Point(243, 0);
			this.TraceListView.Name = "TraceListView";
			this.TraceListView.Size = new System.Drawing.Size(293, 308);
			this.TraceListView.TabIndex = 10;
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
			this.splitter1.Location = new System.Drawing.Point(240, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 308);
			this.splitter1.TabIndex = 9;
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
			this.SourceTree.Size = new System.Drawing.Size(240, 308);
			this.SourceTree.TabIndex = 8;
			this.SourceTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.SourceTree_AfterCheck);
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "trace";
			this.openFileDialog.InitialDirectory = "C:\\Seagate";
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "trace";
			this.saveFileDialog.InitialDirectory = "C:\\Seagate";
			this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
			// 
			// PanelTrace
			// 
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolBar1);
			this.Name = "PanelTrace";
			this.Size = new System.Drawing.Size(536, 336);
			this.VisibleChanged += new System.EventHandler(this.PanelTrace_VisibleChanged);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

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
			TraceListView.BeginUpdate();
			TraceListView.Clear();
			if(checkedNodesList.Count == 0)
			{
				TraceListView.EndUpdate();
				TraceListView.Enabled = false;
				return;
			}
			else
				TraceListView.Enabled = true;

			if(ProcessViewBtn.Pushed)
				PopulateColumns_ProcessView();
			else
				PopulateColumns();

			// autosize the columns
			foreach(ColumnHeader ch in TraceListView.Columns)
				ch.Width = -2;	// autosize
			TraceListView.EndUpdate();
		}

		private void PopulateColumns()
		{
			// add the columns and determine max row count for the list
			int maxRow = 0;
			bool viewTimeStamp = ViewTimeStampBtn.Pushed;
			foreach(TreeNode node in checkedNodesList)
			{
				string header = String.Format("{0}.{1}",node.Parent.Text,node.Text);
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

		private void PopulateColumns_ProcessView()
		{
			bool viewTimeStamp = ViewTimeStampBtn.Pushed;
			// add the columns and determine row count for the list (total # of traces)
			int rowCount = 0;
			int i = 0;
			string [] channel = new string[checkedNodesList.Count];
			int [] index = new int[checkedNodesList.Count];
			foreach(TreeNode node in checkedNodesList)
			{
				string header = String.Format("{0}.{1}",node.Parent.Text,node.Text);
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

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == ProcessViewBtn)
			{
//				ProcessViewBtn.Pushed = !ProcessViewBtn.Pushed;
				PopulateListView();
				return;
			}
			if(e.Button == ViewTimeStampBtn)
			{
//				ViewTimeStampBtn.Pushed = !ViewTimeStampBtn.Pushed;
				PopulateListView();
				return;
			}
			if(e.Button == OpenFileBtn)
			{
				OpenFile();
				return;
			}
			if(e.Button == SaveFileBtn)
			{
				SaveFile();
				return;
			}
			if(e.Button == RefreshTracesBtn)
			{
				Initialize(ServiceManager.Tracing.TraceTable);
				PopulateListView();
				return;
			}
		}

		private void OpenFile()
		{
			// Open the file containing the data that you want to deserialize.
			if(openFileDialog.ShowDialog() != DialogResult.OK)
				return;
			
			string fileName = openFileDialog.FileName;
			FileStream fs = new FileStream(fileName, FileMode.Open);

			try 
			{
				BinaryFormatter formatter = new BinaryFormatter();

				// Deserialize the hashtable from the file and 
				Hashtable traces = (Hashtable) formatter.Deserialize(fs);
				Initialize(traces);
				PopulateListView();
			}
			catch(SerializationException e) 
			{
				Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
				throw;
			}
			finally 
			{
				fs.Close();
			}

		}

		private void SaveFile()
		{
			saveFileDialog.FileName = "Traces_" + DateTime.Now.ToString("yyMMdd") + ".trace";
			if(saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = saveFileDialog.FileName;

				FileStream fs = new FileStream(fileName, FileMode.Create);

				// Construct a BinaryFormatter and use it to serialize the data to the stream.
				BinaryFormatter formatter = new BinaryFormatter();
				try 
				{
					formatter.Serialize(fs, this.ht);
				}
				catch (SerializationException e) 
				{
					MessageBox.Show("Failed to serialize. Reason: " + e.Message);
				}
				finally 
				{
					fs.Close();
				}
			}

		}

		private void saveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
		
		}

		internal bool TracesAreVisible(string source)
		{
			foreach(TreeNode tn in checkedNodesList)
			{
				if(source == tn.FullPath.Replace('\\','.'))
					return true;
			}
			return false;
		}

		private void PanelTrace_VisibleChanged(object sender, EventArgs e)
		{
			// DesignMode doesn't seem to be working ???
			if(ServiceManager.Tracing != null)
				ServiceManager.Tracing.SetPanelTraceVisible(this, this.Visible);
		}

		internal void UpdateTraces()
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(Update));
			else
				PopulateListView();
		}
	}
}
