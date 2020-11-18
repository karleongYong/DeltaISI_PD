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
//  [8/21/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// 
	/// </summary>
    public class PanelIOManifest : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.IContainer components;

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Splitter splitter1;
        private Seagate.AAS.Parsel.Hw.PanelAxis panelAxis1;
        bool       assigned = false;
        private IOManifest ioManifest;

        public enum NodeType
        {
            DigitalInput,
            DigitalOutput,
            AnalogInput,
            AnalogOutput,
            Axis            
        }

        public enum NodeImage
        {
            AmpNode,
            IONode,
            DigitalInput,
            DigitalOutput,
            AnalogInput,
            AnalogOutput,
            Axis,
            On,
            Off,
            Unknown
        }


        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelIOManifest));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelAxis1 = new Seagate.AAS.Parsel.Hw.PanelAxis();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 400;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "AmpNode.bmp");
            this.imageList1.Images.SetKeyName(1, "IONode.bmp");
            this.imageList1.Images.SetKeyName(2, "DigitalInput.bmp");
            this.imageList1.Images.SetKeyName(3, "DigitalOutput.bmp");
            this.imageList1.Images.SetKeyName(4, "AnalogIn.bmp");
            this.imageList1.Images.SetKeyName(5, "AnalogOut.bmp");
            this.imageList1.Images.SetKeyName(6, "Axis.bmp");
            this.imageList1.Images.SetKeyName(7, "On.bmp");
            this.imageList1.Images.SetKeyName(8, "Off.bmp");
            this.imageList1.Images.SetKeyName(9, "Unknown.bmp");
            // 
            // listView1
            // 
            this.listView1.FullRowSelect = true;
            this.listView1.LabelEdit = true;
            this.listView1.Location = new System.Drawing.Point(176, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(376, 104);
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Visible = false;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView1_AfterLabelEdit);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(152, 328);
            this.treeView1.TabIndex = 3;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(152, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(8, 328);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // panelAxis1
            // 
            this.panelAxis1.Location = new System.Drawing.Point(176, 112);
            this.panelAxis1.Name = "panelAxis1";
            this.panelAxis1.Size = new System.Drawing.Size(376, 200);
            this.panelAxis1.TabIndex = 5;
            this.panelAxis1.Visible = false;
            // 
            // PanelIOManifest
            // 
            this.Controls.Add(this.panelAxis1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PanelIOManifest";
            this.Size = new System.Drawing.Size(584, 328);
            this.VisibleChanged += new System.EventHandler(this.PanelIOManifest_VisibleChanged);
            this.Load += new System.EventHandler(this.PanelIOList_Load);
            this.EnabledChanged += new System.EventHandler(this.PanelIOManifest_EnabledChanged);
            this.ResumeLayout(false);

        }
	
        public PanelIOManifest()
        {
            InitializeComponent();

            panelAxis1.Font = this.Font;
        }
		
        protected virtual void timer1_Tick(object sender, System.EventArgs e)
        {
            if (!assigned) return;

            if (this.Visible == false)
            {
                timer1.Enabled = false;
                return;
            }

            if (treeView1.SelectedNode == null) return;

            NodeType nodeType = (NodeType) treeView1.SelectedNode.Tag;

            if (nodeType == NodeType.Axis) return;

            listView1.BeginUpdate();
            DigitalIOState state;
            switch (nodeType)
            {
                case NodeType.DigitalInput:
                    foreach (ListViewItem item in listView1.Items) 
                    {
                        state = ((IDigitalInput) item.Tag).Get();
                        item.SubItems[0].Text = ( state == DigitalIOState.On)?"ON ":"OFF";
                    }
                break;

                case NodeType.DigitalOutput:
                    foreach (ListViewItem item in listView1.Items) 
                    {
                        state = ((IDigitalOutput) item.Tag).Get();
                        item.SubItems[0].Text = ( state == DigitalIOState.On)?"ON ":"OFF";
                    }
                    break;

                case NodeType.AnalogInput:
                    foreach (ListViewItem item in listView1.Items) 
                    {
                        item.SubItems[0].Text = ((IAnalogInput) item.Tag).Get().ToString();
                    }
                    break;

                case NodeType.AnalogOutput:
                    foreach (ListViewItem item in listView1.Items) 
                    {
                        item.SubItems[0].Text = ((IAnalogOutput) item.Tag).Get().ToString();
                    }
                    break;
            }
            listView1.EndUpdate();
        }

        private void FillTreeView()
        {

        
            // Suppress repainting the TreeView until all the objects have been created.
            treeView1.BeginUpdate();

            // Clear the TreeView each time the method is called.
            treeView1.Nodes.Clear();
					
            if (assigned) 
            {				

                // create node for each IO type
                if (ioManifest.digitalInputMap != null && ioManifest.digitalInputMap.Count > 0) 
                {
                    TreeNode node = new TreeNode("Digital Inputs");
                    node.Tag = NodeType.DigitalInput;
                    node.ImageIndex = (int)NodeImage.DigitalInput;
                    node.SelectedImageIndex = (int)NodeImage.DigitalInput;
                    treeView1.Nodes.Add(node);
                }

                if (ioManifest.digitalOutputMap != null && ioManifest.digitalOutputMap.Count > 0) 
                {
                    TreeNode node = new TreeNode("Digital Output");
                    node.Tag = NodeType.DigitalOutput;
                    node.ImageIndex = (int)NodeImage.DigitalOutput;
                    node.SelectedImageIndex = (int)NodeImage.DigitalOutput;
                    treeView1.Nodes.Add(node);
                }

                if (ioManifest.analogInputMap != null && ioManifest.analogInputMap.Count > 0) 
                {
                    TreeNode node = new TreeNode("Analog In");
                    node.Tag = NodeType.AnalogInput;
                    node.ImageIndex = (int)NodeImage.AnalogInput;
                    node.SelectedImageIndex = (int)NodeImage.AnalogInput;
                    treeView1.Nodes.Add(node);
                }

                if (ioManifest.analogOutputMap != null && ioManifest.analogOutputMap.Count > 0) 
                {
                    TreeNode node = new TreeNode("Analog Out");
                    node.Tag = NodeType.AnalogOutput;
                    node.ImageIndex = (int)NodeImage.AnalogOutput;
                    node.SelectedImageIndex = (int)NodeImage.AnalogOutput;
                    treeView1.Nodes.Add(node);
                }

                if (ioManifest.axisMap != null && ioManifest.axisMap.Count > 0) 
                {
                    TreeNode node = new TreeNode("Axis");
                    node.Tag = NodeType.Axis;
                    node.ImageIndex = (int)NodeImage.Axis;
                    node.SelectedImageIndex = (int)NodeImage.Axis;
                    treeView1.Nodes.Add(node);
                }

                // format the display
                treeView1.ExpandAll();
                bool selected = false;
                foreach (TreeNode node in treeView1.Nodes)
                {
                    foreach (TreeNode subNode in node.Nodes) 
                    {
                        if (!selected) 
                        {
                            // select first node
                            treeView1.SelectedNode = subNode;
                            selected = true;
                            break;
                        }
                    }
                    if (selected)
                        break;
                }
            }
            else
            {
                TreeNode node = new TreeNode("IO Manifest not assigned yet");
                node.ImageIndex = (int)NodeImage.Unknown;
                node.SelectedImageIndex = (int)NodeImage.Unknown;
                treeView1.Nodes.Add(node);
            }


            // Begin repainting the TreeView.
            treeView1.EndUpdate();
        }


        private void PanelIOList_Load(object sender, System.EventArgs e)
        {
            FillTreeView();
        }


        protected virtual void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            NodeType ioType = (NodeType) node.Tag;

            listView1.BeginUpdate();
            listView1.Clear();
            listView1.View = View.Details;

            switch(ioType) 
            {
                case NodeType.DigitalInput:
                    listView1.Columns.Add("State"             ,  70, HorizontalAlignment.Center);
                    listView1.Columns.Add("Digital Input Name", 300, HorizontalAlignment.Left);
                    foreach (IDigitalInput ioPoint in ioManifest.digitalInputMap.Values) 
                    {
                        ListViewItem item = new ListViewItem((ioPoint.State == DigitalIOState.On)?"ON ":"OFF", 0);
                        item.Tag = ioPoint;
                        item.SubItems.Add(ioPoint.Name);
                        listView1.Items.Insert(0, item);
                    }
                    break;
                case NodeType.DigitalOutput:
                    listView1.Columns.Add("State"              ,  70, HorizontalAlignment.Center);
                    listView1.Columns.Add("Digital Output Name", 300, HorizontalAlignment.Left);

                    foreach (IDigitalOutput ioPoint in ioManifest.digitalOutputMap.Values) 
                    {
                        ListViewItem item = new ListViewItem((ioPoint.State == DigitalIOState.On)?"ON ":"OFF", 0);
                        item.Tag = ioPoint;
                        item.SubItems.Add(ioPoint.Name);
                        listView1.Items.Insert(0, item);
                    }
                    break;              
                case NodeType.AnalogInput:
                    listView1.Columns.Add("Value"            , 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Unit"             , 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Analog Input Name", 250, HorizontalAlignment.Left);

                    foreach (IAnalogInput ioPoint in ioManifest.analogInputMap.Values) 
                    {
                        ListViewItem item = new ListViewItem(ioPoint.Value.ToString(), 0);
                        item.Tag = ioPoint;
                        item.SubItems.Add(ioPoint.Unit);
                        item.SubItems.Add(ioPoint.Name);
                        listView1.Items.Insert(0, item);
                    }
                    break;   
                case NodeType.AnalogOutput:
                    listView1.Columns.Add("Value"             , 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Unit"              , 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Analog Output Name", 250, HorizontalAlignment.Left);
                    foreach (IAnalogOutput ioPoint in ioManifest.analogOutputMap.Values) 
                    {
                        ListViewItem item = new ListViewItem(ioPoint.Value.ToString(), 0);
                        item.Tag = ioPoint;
                        item.SubItems.Add(ioPoint.Unit);
                        item.SubItems.Add(ioPoint.Name);
                        listView1.Items.Insert(0, item);
                    }
                    break;   
            }

            listView1.EndUpdate();

            if (ioType == NodeType.Axis)
            {
                listView1.Visible = false;
                listView1.Dock = DockStyle.None;

                panelAxis1.Enabled = true;
                panelAxis1.Visible = true;
                panelAxis1.Dock = DockStyle.Fill;

                // assign the one and only axis for this node
                panelAxis1.AssignAxisMap(ioManifest.axisMap);

            }
            else 
            {
                listView1.Visible = true;
                listView1.Dock = DockStyle.Fill;

                panelAxis1.Enabled = false;
                panelAxis1.Visible = false;
                panelAxis1.Dock = DockStyle.Fill;
                panelAxis1.AssignAxis(null);

            }

        }

        protected virtual void listView1_DoubleClick(object sender, System.EventArgs e)
        {
            ListViewItem item = listView1.SelectedItems[0];
            if(item.Tag is IDigitalOutput)
            {
                IDigitalOutput digOut = (IDigitalOutput) item.Tag;
                digOut.Toggle();
            }
            else if (item.Tag is IAnalogOutput)
            {
                IAnalogOutput analogOut = (IAnalogOutput) item.Tag;
                item.BeginEdit();
            }
        }

        private void listView1_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {

        }

        private void listView1_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
        {
            ListViewItem item = listView1.SelectedItems[0];
            if(item.Tag is IAnalogOutput)
            {
                IAnalogOutput analogOut = (IAnalogOutput) item.Tag;
                if (e.Label != null)
                {
                    double newValue = Convert.ToDouble(e.Label);
                    analogOut.Set(newValue);
                }

            }
        }

        public void AssignIOManifest(IOManifest ioManifest)
        {
            this.ioManifest = ioManifest;
            assigned = true;
            FillTreeView();
        }

        private void PanelIOManifest_EnabledChanged(object sender, System.EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                control.Enabled = this.Enabled;  
            }

            //timer1.Enabled = this.Enabled;       
            
            if (this.Enabled == false)
            {
                panelAxis1.AssignAxis(null);
            }

        }

        private void PanelIOManifest_VisibleChanged(object sender, System.EventArgs e)
        {
            timer1.Enabled = this.Visible;
        }

    }
}
