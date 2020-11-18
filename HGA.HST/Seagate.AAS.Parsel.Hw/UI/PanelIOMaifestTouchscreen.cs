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
    public class PanelIOManifestTouchscreen : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.IContainer components;

        private System.Windows.Forms.ImageList imageList1;
		private Seagate.AAS.Parsel.Hw.PanelIO panelIO1;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelIOManifestTouchscreen));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelAxis1 = new Seagate.AAS.Parsel.Hw.PanelAxis();
            this.panelIO1 = new Seagate.AAS.Parsel.Hw.PanelIO();
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
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.HideSelection = false;
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
            this.panelAxis1.Location = new System.Drawing.Point(184, 112);
            this.panelAxis1.Name = "panelAxis1";
            this.panelAxis1.Size = new System.Drawing.Size(376, 200);
            this.panelAxis1.TabIndex = 5;
            this.panelAxis1.Visible = false;
            // 
            // panelIO1
            // 
            this.panelIO1.AutoScroll = true;
            this.panelIO1.Location = new System.Drawing.Point(200, 32);
            this.panelIO1.Name = "panelIO1";
            this.panelIO1.Size = new System.Drawing.Size(150, 150);
            this.panelIO1.SizeAnalog = new System.Drawing.Size(200, 72);
            this.panelIO1.SizeLed = new System.Drawing.Size(136, 42);
            this.panelIO1.TabIndex = 6;
            // 
            // PanelIOManifestTouchscreen
            // 
            this.Controls.Add(this.panelIO1);
            this.Controls.Add(this.panelAxis1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PanelIOManifestTouchscreen";
            this.Size = new System.Drawing.Size(584, 328);
            this.Load += new System.EventHandler(this.PanelIOList_Load);
            this.EnabledChanged += new System.EventHandler(this.PanelIOManifest_EnabledChanged);
            this.VisibleChanged += new System.EventHandler(this.PanelIOManifestTouchscreen_VisibleChanged);
            this.ResumeLayout(false);

		}
	
        public PanelIOManifestTouchscreen()
        {
            InitializeComponent();

            panelAxis1.Font = this.Font;
        }

        public int UpdateInterval
        {
            set { timer1.Interval = value; }
        }
		
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (this.Visible == false)
            {
                timer1.Enabled = false;
                return;
            }

            if (!assigned) return;

            if (treeView1.SelectedNode == null) return;

            timer1.Enabled = false;
            NodeType nodeType = (NodeType)treeView1.SelectedNode.Tag; 
            
            if (nodeType == NodeType.Axis) return;

			panelIO1.UpdateStates();
            //double et = ((TimeSpan)(DateTime.Now - start)).TotalMilliseconds;
            timer1.Enabled = true;
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


		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			TreeNode node = treeView1.SelectedNode;
			//			if(node.Parent != null)
			//			{
			panelIO1.Clear();
			NodeType ioType = (NodeType) node.Tag;
			string label;
			
			switch(ioType) 
			{
				case NodeType.DigitalInput:
					foreach (IDigitalInput ioPoint in ioManifest.digitalInputMap.Values) 
					{
						label = ioPoint.Name;
						panelIO1.AddIO(ioPoint, label);
					}
					break;
				case NodeType.DigitalOutput:
					foreach (IDigitalOutput ioPoint in ioManifest.digitalOutputMap.Values) 
					{
						label = ioPoint.Name;
						panelIO1.AddIO(ioPoint, label);
					}
					break;              
				case NodeType.AnalogInput:
					foreach (IAnalogInput ioPoint in ioManifest.analogInputMap.Values) 
					{
						label = ioPoint.Name;
						panelIO1.AddIO(ioPoint, label);
					}
					break;   
				case NodeType.AnalogOutput:
					foreach (IAnalogOutput ioPoint in ioManifest.analogOutputMap.Values) 
					{
						label = ioPoint.Name;
						panelIO1.AddIO(ioPoint, label);
					}
					break;   
			}

			if (ioType == NodeType.Axis)
			{
				panelIO1.Visible = false;
				panelIO1.Dock = DockStyle.None;

				panelAxis1.Enabled = true;
				panelAxis1.Visible = true;
				panelAxis1.Dock = DockStyle.Fill;

				// assign the one and only axis for this node
				panelAxis1.AssignAxisMap(ioManifest.axisMap);

			}
			else 
			{
				panelIO1.Visible = true;
				panelIO1.Dock = DockStyle.Fill;
				panelIO1.UpdateLayout();

				panelAxis1.Enabled = false;
				panelAxis1.Visible = false;
				panelAxis1.Dock = DockStyle.Fill;
				panelAxis1.AssignAxis(null);

			}
			//			}
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

            timer1.Enabled = this.Enabled;       
            
            if (this.Enabled == false)
            {
                panelAxis1.AssignAxis(null);
            }

        }

		private void PanelIOManifestTouchscreen_VisibleChanged(object sender, System.EventArgs e)
		{
			if(!this.DesignMode)
			{
				timer1.Enabled = this.Visible;
			}
		}

    }
}
